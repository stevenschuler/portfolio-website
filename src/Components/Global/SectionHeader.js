import React, {useEffect, useState} from "react";
import "../../Styles/global.css";
import PropTypes from "prop-types";

const SectionHeader = ({headerText, alignLeft}) => {
    const cn = `section-header-${alignLeft ? 'left' : 'right'}` 
    console.log(cn)
  return (
    <div className={cn}>
        <h1>{headerText}</h1>
    </div>
  );
};

SectionHeader.propTypes = {
    headerText: PropTypes.string.isRequired,
    alignLeft: PropTypes.bool.isRequired,
  };

export default SectionHeader;