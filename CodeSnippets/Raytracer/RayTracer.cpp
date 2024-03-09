#include <iostream>
#include <fstream>
#include <glm/glm.hpp> // Graphics library - functions to handle vectors, matrices, etc
#include "scene.h" // Custom library - contains classes for sphere, light, camera/scene
#include "invert.h"


using namespace std;


// Function used for debugging to ensure proper reading of input file
void describeScene(scene::Sphere spheres[], scene::Light lights[], glm::vec3 backgroundRGB, glm::vec3 ambientRGB, string outputFileName,
	float near, float top, float bottom, float left, float right, int width, int height, int sphereIndex, int lightIndex);
void save_imageP3(int Width, int Height, char* fname, unsigned char* pixels);
void save_imageP6(int Width, int Height, char* fname, unsigned char* pixels);
float check_collision(scene::ray r, scene::Sphere sphere, float near);
glm::vec3 calculate_colour(int depth, scene::ray r, scene::Sphere spheres[], int num_spheres, scene::Light lights[], int num_lights, float near, glm::vec3 ambient);
glm::vec3 shadow_ray(glm::vec3 hitpoint, scene::Sphere spheres[], int numSpheres, scene::Light lights[], int numLights, glm::vec3 ambient, float near);

int hit = 0; // Used to debug to see how many pixels were hit and not hit
int nothit = 0;


int main(int argc, char* argv[])
{
	// Check for proper program arguments
	if (argc < 2) {
		printf("ERROR: Expected the name of exactly one text file as program argument.\n");
		exit(EXIT_FAILURE);
	}
	
	// Try to open file provided
	fstream file;
	file.open(argv[1], ios::in);
	if (!file) {
		cout << "File not found. Please try again.";
		exit(EXIT_FAILURE);
	}

	// camera/scene properties
	char outputFileName[30] = ""; for (int i = 0; i < 30; i++) { outputFileName[i] = '\0'; }
	float near = 0, left = 0, right = 0, bottom = 0, top = 0, bgR = 0, bgG = 0, bgB = 0, ambientR = 0, ambientG = 0, ambientB = 0;
	int width = 0, height = 0;
	glm::vec3 backgroundRGB = glm::vec3(0.0,0.0,0.0), ambientRGB = glm::vec3(0.0f, 0.0f, 0.0f), pixel_colour = glm::vec3(0.0f, 0.0f, 0.0f);

	// Sphere data
	string sphereName;
	float sphereX, sphereY, sphereZ, scaleX, scaleY, scaleZ, sphereR, sphereG, sphereB, ka, kd, ks, kr, n;
	int num_spheres = 0;

	// Light data
	string lightName;
	float lightX, lightY, lightZ, lightR, lightG, lightB;
	int num_lights = 0;

	scene::Sphere spheres[15]; // Up to 15 spheres
	int sphereIndex = 0;

	scene::Light lights[10]; // Up to 10 lights
	int lightIndex = 0;

	scene::Camera cam; // Scene camera (only 1)

	string check; 

	// Check each line until end of file
	// The check being checked in the while loop condition is the keyword to be checked (or blank line)
	while (file >> check) {
		if      (check == "NEAR") { file >> near; }
		else if (check == "LEFT") { file >> left; }
		else if (check == "RIGHT") { file >> right; }
		else if (check == "TOP") { file >> top; }
		else if (check == "BOTTOM") { file >> bottom; }
		else if (check == "RES") { file >> width; file >> height; }
		else if (check == "OUTPUT") { file >> outputFileName; }
		else if (check == "BACK") {
			file >> bgR; file >> bgG; file >> bgB;
			backgroundRGB = glm::vec3(bgR, bgG, bgB);
		}
		else if (check == "AMBIENT") {
			file >> ambientR; file >> ambientG;  file >> ambientB;
			ambientRGB = glm::vec3(ambientR, ambientG, ambientB);
		}
		else if (check == "SPHERE") {
			num_spheres++;
			file >> sphereName; file >> sphereX; file >> sphereY; file >> sphereZ;  
			file >> scaleX; file >> scaleY; file >> scaleZ; 
			file >> sphereR; file >> sphereG; file >> sphereB;
			file >> ka; file >> kd; file >> ks; file >> kr; file >> n;

			// Create position vec3
			glm::vec4 vecPos = glm::vec4(sphereX, sphereY, sphereZ, 0);
			// Create scale vec3
			glm::vec3 vecScale = glm::vec3(scaleX, scaleY, scaleZ);
			// Create rgb vec3
			glm::vec3 vecRGB = glm::vec3(sphereR, sphereG, sphereB);
			// Create a sphere object from the data gathered
			spheres[sphereIndex].addAllSphereData(sphereName, vecPos, vecScale, vecRGB, ka, kd, ks, kr, n);
			sphereIndex++;
		}
		else if (check == "LIGHT") {
			num_lights++;
			file >> lightName;
			file >> lightX; file >> lightY; file >> lightZ;
			file >> lightR; file >> lightG; file >> lightB;
			// Create position vec3
			glm::vec4 vecPos = glm::vec4(lightX, lightY, lightZ, 0);
			// Create rgb vec3
			glm::vec3 vecRGB = glm::vec3(lightR, lightG, lightB);
			// Create a light object from the data gathered
			lights[lightIndex].addAllLightData(lightName, vecPos, vecRGB);
			lightIndex++;
		}	
	}

	// Debugging function
	describeScene(spheres, lights, backgroundRGB, ambientRGB, outputFileName, near, top, bottom, left, right, width, height, sphereIndex, lightIndex);

	unsigned char* pixels = new unsigned char[3 * width * height];
	int k = 0;
	float hit_distance = 0;
	
	glm::vec3 eye = glm::vec3(0, 0, 0);

	// Loop through each pixel
	for (int y = 0; y < height; y++) {
		for (int x = 0; x < width; x++) {
			pixel_colour = glm::vec3(0.0f, 0.0f, 0.0f);
			// Calculate the ray at this pixel location from eye -> pixel location on the near plane
			scene::ray r;
			r.rayOrigin = eye;

			// pixel coordinates scaled by the dimensions of the screen and multiplied by the clipping coordinates
			float fx = ((((float)x - ((float)width) / 2.0) / (float)width)) * (left - right);
			float fy = ((((float)y - ((float)height) / 2.0) / (float)height)) * (top - bottom);
			
			r.rayVector = glm::vec3(-fx, -fy, -near);

			r.originMat = glm::mat4x4(
				1.0, 0.0, 0.0, r.rayOrigin.x,
				0.0, 1.0, 0.0, r.rayOrigin.y,
				0.0, 0.0, 1.0, r.rayOrigin.z,
				0.0, 0.0, 0.0, 1.0);
			r.rayMat = glm::mat4x4(
				1.0, 0.0, 0.0, r.rayVector.x,
				0.0, 1.0, 0.0, r.rayVector.y,
				0.0, 0.0, 1.0, r.rayVector.z,
				0.0, 0.0, 0.0, 0.0);

			// Recursive calculation function
			pixel_colour = calculate_colour(0, r, spheres, num_spheres, lights, num_lights, near, ambientRGB);

			// If no colour returned (this way to deal with floating point errors)
			if (pixel_colour.r < 0.01 && pixel_colour.g < 0.01 && pixel_colour.b < 0.01) {
				pixels[k] = (int)(backgroundRGB.r * 255.0);
				pixels[k + 1] = (int) (backgroundRGB.g * 255.0);
				pixels[k + 2] = (int) (backgroundRGB.b * 255.0);
			}
			// If calculate_colour returned a colour, scale and set that pixel
			else {
				pixels[k] = (int)(glm::min(255.0f, pixel_colour.r ) * 255);
				pixels[k + 1] = (int)(glm::min(255.0f, pixel_colour.g ) * 255);
				pixels[k + 2] = (int)(glm::min(255.0f, pixel_colour.b) * 255);
			}	
			k += 3;
		}
	}

	save_imageP3(width, height, outputFileName, pixels);
	
	file.close();
	return 0;
}

// Checks if ray r and sphere intersect. Returns distance h, or -1 if no intersection
float check_collision(scene::ray r, scene::Sphere sphere, float near) {
	float h = -1.0f;
	float hit_check = -1.0f;

	// Inverse transformations
	glm::vec3 tp = r.rayOrigin - sphere.posVec3;
	glm::vec3 tr = r.rayVector - sphere.posVec3;
	tp = tp / sphere.scaleVec;
	tr = tr / sphere.scaleVec;

	tr -= tp;

	glm::vec3 trFinal = glm::vec3(tr.x, tr.y, tr.z);

	// Quadratic formula
	float A = glm::dot(trFinal, trFinal); // |c|^2
	float B = glm::dot(tp, trFinal); // (S * C)
	float C = glm::dot(tp, tp); // |S|^2 (-1 applied later)

	float discr = (B * B) - (A * (C - 1.0f));

	if (discr > 0.0f) {
		float ans1 = (-B / A) - (glm::sqrt(discr) / A);
		float ans2 = (-B / A) + (glm::sqrt(discr) / A);
		h = glm::min(ans1, ans2);

	}

	return h ;
}


// Calculates the local lighting at a given hitpoint based on how many light sources are visible from that spot
glm::vec3 shadow_ray(glm::vec3 hitpoint, scene::Sphere spheres[], int numSpheres, scene::Light lights[], int numLights, glm::vec3 ambient, float near) {
	float hitdistance = -1;

	glm::vec3 colour = glm::vec3(0.0f, 0.0f, 0.0f);

	int collision_sphere_index = -1;
	// Loop over all lights
	for (int p = 0; p < numLights; p++) {
		// set shadow ray r
		scene::ray r;
		r.rayVector = lights[p].posVec - hitpoint;
		r.rayOrigin = hitpoint;

		hitdistance = -1.0f;

		// Loop over all spheres, find the closest intersection (otherwise, return -1)
		for (int i = 0; i < numSpheres; i++) {

			float h = check_collision(r, spheres[i], near);

			// If hit, set the hitdistance to the minimum between newhit and previous hit
			if (h > -0.5) {
				if (hitdistance < -0.5) { hitdistance = h; collision_sphere_index = i; }
				else { if (h < hitdistance) { hitdistance = h; collision_sphere_index = i; } }
			}
		}

		// collision found, this light is not visible from this location
		if (hitdistance < -0.5) {
			continue;
		}

		// get inverse transpose of hitpoint's normal
		glm::vec3 tSP = hitpoint - spheres[collision_sphere_index].posVec3;
		glm::vec3 tR = lights[p].posVec - spheres[collision_sphere_index].posVec3;

		tSP = tSP / spheres[collision_sphere_index].scaleVec;
		tR = tR / spheres[collision_sphere_index].scaleVec;

		//tR -= tSP;

		tR = normalize(tR);

		float nl = glm::dot(tR, tSP);

		// No collision found, add the diffuse colour 
		colour += glm::vec3(
			lights[p].colourVec.r * spheres[collision_sphere_index].colourVec.r * spheres[collision_sphere_index].kd * nl,
			lights[p].colourVec.g * spheres[collision_sphere_index].colourVec.g * spheres[collision_sphere_index].kd * nl,
			lights[p].colourVec.b * spheres[collision_sphere_index].colourVec.b * spheres[collision_sphere_index].kd * nl);
	}

	// Add ambient light
	colour += glm::vec3(
		ambient.r * spheres[collision_sphere_index].colourVec.r * spheres[collision_sphere_index].ka,
		ambient.g * spheres[collision_sphere_index].colourVec.g * spheres[collision_sphere_index].ka,
		ambient.b * spheres[collision_sphere_index].colourVec.b * spheres[collision_sphere_index].ka);
	return colour;
}


// Input: current recursion depth, sphere that has been colided, the ray, and all the lights in the scene
// Output: The colour value for the particular pixel
glm::vec3 calculate_colour(int depth, scene::ray r, scene::Sphere spheres[], int num_spheres, scene::Light lights[], int num_lights, float near, glm::vec3 ambient) {
	// When the depth hits 4, return black and end recursion
	if (depth == 3) {
		return glm::vec3(0, 0, 0);
	}

	glm::vec3 colour = glm::vec3(0.0, 0.0, 0.0), local_colour = glm::vec3(0.0, 0.0, 0.0), refl_colour = glm::vec3(0.0, 0.0, 0.0), refr_colour = glm::vec3(0.0, 0.0, 0.0);
	float hitdistance = -1;
	int collision_sphere_index = -1;

	// Loop over all spheres, find the closest intersection (otherwise, return -1)
	for (int p = 0; p < num_spheres; p++) {
		float h = check_collision(r, spheres[p], near);
		
		// If no hit has been found for this ray yet
		if (hitdistance < -0.5) {
			// If a hit is found, set it as the new closest hit
			if (h > -0.5) {
				hitdistance = h;
				collision_sphere_index = p;
			}
		}
		// If a ray collides with multiple spheres, only take the closest distance
		else if ( h > -0.5) {
			hitdistance = glm::min(hitdistance, h);
			collision_sphere_index = p;
		}
	}

	// if a hit was detected, continue the colour calculation
	if (hitdistance > -0.5) {
		hit++;
	}
	// If no hit, skip this object
	else {
		nothit++;
		return glm::vec3(0.0, 0.0, 0.0);
	}

	glm::vec3 hitpoint = r.rayOrigin + ((hitdistance - 0.0001f) * r.rayVector);

	// calculate shadow rays
	glm::vec3 colour_temp = shadow_ray(hitpoint, spheres, num_spheres, lights, num_lights, ambient, near);
	local_colour.r += colour_temp.r;
	local_colour.g += colour_temp.g;
	local_colour.b += colour_temp.b;


	// Compute reflection ray - recursion. Increment depth
	// Get spheres normal vector
	// get normal of hitpoint

	glm::vec3 tSP = hitpoint - spheres[collision_sphere_index].posVec3;
	glm::vec3 tR = r.rayVector - hitpoint - spheres[collision_sphere_index].posVec3;

	tSP = tSP / spheres[collision_sphere_index].scaleVec;
	tR = tR / spheres[collision_sphere_index].scaleVec;

	tR -= tSP;

	tR = normalize(tR);

	glm::vec3 refl_ray = (- 2.0f * (glm::dot(tR, r.rayVector)) * tR) + r.rayVector;

	scene::ray newr;
	newr.rayVector = refl_ray;
	newr.rayOrigin = hitpoint;

	// Compute reflection ray (not implemented currently)
	// refl_colour = calculate_colour(depth + 1, newr, spheres, num_spheres, lights, num_lights, near, ambient);

	// Finally, add all 3 light sources together to get final value. return this value
	//colour += spheres[collision_sphere_index].kr * refl_colour;
	colour += local_colour;

	return colour;
}


void save_imageP3(int Width, int Height, char* fname, unsigned char* pixels) {
	FILE* fp;
	const int maxVal = 255;

	printf("Saving image %s: %d x %d\n", fname, Width, Height);
	fp = fopen(fname, "w");
	if (!fp) {
		printf("Unable to open file '%s'\n", fname);
		return;
	}
	fprintf(fp, "P3\n");
	fprintf(fp, "%d %d\n", Width, Height);
	fprintf(fp, "%d\n", maxVal);

	int k = 0;
	for (int j = 0; j < Height; j++) {

		for (int i = 0; i < Width; i++)
		{
			fprintf(fp, " %d %d %d", pixels[k], pixels[k + 1], pixels[k + 2]);
			k = k + 3;
		}
		fprintf(fp, "\n");
	}
	fclose(fp);
}


void save_imageP6(int Width, int Height, char* fname, unsigned char* pixels) {
	FILE* fp;
	const int maxVal = 255;

	printf("Saving image %s: %d x %d\n", fname, Width, Height);
	fp = fopen(fname, "wb");
	if (!fp) {
		printf("Unable to open file '%s'\n", fname);
		return;
	}
	fprintf(fp, "P6\n");
	fprintf(fp, "%d %d\n", Width, Height);
	fprintf(fp, "%d\n", maxVal);

	for (int j = 0; j < Height; j++) {
		fwrite(&pixels[j * Width * 3], 3, Width, fp);
	}

	fclose(fp);
}


// Function prints the contents of the read file. Used for debugging to ensure proper reading of input file.
void describeScene(scene::Sphere spheres[], scene::Light lights[], glm::vec3 backgroundRGB, glm::vec3 ambientRGB, string outputFileName,
	float near, float top, float bottom, float left, float right, int width, int height, int sphereIndex, int lightIndex) 
{
	cout << "The output file name will be: " << outputFileName << endl;
	cout << "scene info: ";
	cout << "Near " << near << ", left " << left << ", right " << right << ", top " << top << ", bottom " << bottom << endl;
	cout << "Resolution: width " << width << ", height " << height << endl;
	cout << "Ambient light: (" << ambientRGB.r << " " << ambientRGB.g << " " << ambientRGB.b << ")\n";
	cout << "Background colour: (" << backgroundRGB.r << " " << backgroundRGB.g << " " << backgroundRGB.b << ")\n";

	cout << "There are " << sphereIndex << " spheres and " << lightIndex << " lights in the scene.\n";
	cout << "Sphere location(s) + colour(s): \n";

	for (int i = 0; i < sphereIndex; i++) {
		cout << spheres[i].name << " position: (" << spheres[i].posVec.x << " " << spheres[i].posVec.y << " " << spheres[i].posVec.z << ") ";
		cout << spheres[i].name << " scale: (" << spheres[i].scaleVec.x << " " << spheres[i].scaleVec.y << " " << spheres[i].scaleVec.z << ") ";
		cout << spheres[i].name << " colour: (" << spheres[i].colourVec.r << " " << spheres[i].colourVec.g << " " << spheres[i].colourVec.b << ")\n";
	}

	cout << "Light location(s) + colour(s): \n";
	for (int i = 0; i < lightIndex; i++) {
		cout << lights[i].name << " position: (" << lights[i].posVec.x << " " << lights[i].posVec.y << " " << lights[i].posVec.z << ") ";
		cout << lights[i].name << " colour: (" << lights[i].colourVec.r << " " << lights[i].colourVec.g << " " << lights[i].colourVec.b << ")\n";
	}

}