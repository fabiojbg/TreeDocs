import React, { Component } from 'react';
import { Button, Form, FormGroup, Input, Label } from 'reactstrap';
import { Navigate } from 'react-router-dom';

export class Login extends Component {

    constructor(props) {
        super(props);
        this.state = {
            userData: localStorage.getItem('UserData'),
            userEmail: "",
            userPassword: "",
            errorMessage: "",
            validate: {
                emailState: '',
            }
        };
        this.handleChange = this.handleChange.bind(this);
    }

    async Login() {
        const requestOptions = {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ UserEmail: this.state.userEmail, Password: this.state.userPassword })
        };
        const response = await fetch("http://localhost:13889/Api/User/Login", requestOptions);
        if (!response.ok) {
            this.setState({ errorMessage: "Erro logando usuário " + this.state.userEmail });
        }
        else {
            const data = await response.json();
            localStorage.setItem('UserData', JSON.stringify(data));
            localStorage.setItem('UserToken', data.Token);
            this.setState({ errorMessage: "Ok" });
            if( this.props.redirectPath != null)
                <Navigate to={this.props.redirectPath} />
            else
                <Navigate to='/home' />
        }        
    }
    render() {
        const { userEmail, userPassword } = this.state;
        return (
            <div>
                <h1>Login</h1>
                <Form>
                    <FormGroup>
                        <Label for="userEmail">User Email</Label>
                        <Input type="email" name="userEmail" id="userEmail" placeholder="Your user email"
                            valid={this.state.validate.emailState === "has-success"}
                            invalid={this.state.validate.emailState === "has-danger"}
                            value={userEmail}
                            onChange={(e) => {
                                this.validateEmail(e);
                                this.handleChange(e);
                            }} />
                    </FormGroup>
                    <FormGroup>
                        <Label for="userPassword">Password</Label>
                        <Input type="password" name="userPassword" id="userPassword" placeholder="Password"
                            value={userPassword}
                            onChange={(e) => this.handleChange(e)} />

                        <p>{this.state.errorMessage}</p>
                    </FormGroup>
                    <Button color="primary" onClick={() => this.Login()}>Login</Button>
                </Form>
            </div>
        );
    }
    handleChange = (event) => {
        const { target } = event;
        const value = target.type === 'checkbox' ? target.checked : target.value;
        const { name } = target;

        this.setState({
            [name]: value,
        });
    };

    validateEmail(e) {
        const emailRex =
            /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;

        const { validate } = this.state;

        if (emailRex.test(e.target.value)) {
            validate.emailState = 'has-success';
        } else {
            validate.emailState = 'has-danger';
        }

        this.setState({ validate });
    }

}
