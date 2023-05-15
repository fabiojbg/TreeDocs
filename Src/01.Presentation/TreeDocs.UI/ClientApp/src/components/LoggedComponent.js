import React, { Component } from 'react';
import { Navigate, withRouter } from 'react-router-dom';


function IsLogged() {
    return localStorage.getItem('UserToken') != null;
}

const Login = () => 

class LoggedComponent extends Component {

    constructor(props) {
        super(props);
    }

    IsLogged() {
        return localStorage.getItem('UserToken') != null;
    }

    render() {
        if (!this.IsLogged()) {
            localStorage.setItem("LoginReturnPath", this.props.location.pathname);
            return <Navigate to="/login" />;
        }
    }
}
export default LoggedComponent;