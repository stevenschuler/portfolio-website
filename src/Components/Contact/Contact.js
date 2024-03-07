import React, {useEffect, useState} from "react";
import "./Contact.css";
import contactData from "../Contact/contact.json";
import SectionHeader from "../../Components/Global/SectionHeader"
import PropTypes from "prop-types";

const Contact = ({backgroundColor}) => {
    const bgc = {
        backgroundColor
    }

  return (
    <div className="contact-section" style={bgc}>
        <SectionHeader headerText={"Contact Information"} alignLeft={true} />
        <p> {contactData.words} </p>
        <h2> {contactData.email} </h2>
    </div>
  );
};

Contact.propTypes = {
    backgroundColor: PropTypes.string,
  };

export default Contact;