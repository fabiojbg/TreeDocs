import React, { Component } from 'react';
import { Navigate } from 'react-router-dom';


export class Logout extends Component {

    constructor(props) {
        super(props);
    }
    
    render() {
        localStorage.removeItem('UserToken');
        return <Navigate to="/login" />;
    }

}
