import React, { Component } from 'react';
import Button from "reactstrap/lib/Button";

export class Authentication extends Component
{
    static displayName = Authentication.name;

    constructor(props) {
        super(props);
        this.state = { authenticated: false };
    }

    googleAuthenticationClick(event)
    {
        window.location.href = "authentication/google-login";
    }
    
    render()
    {
        return(
            <div>
                <h1>Authentication</h1>
                <Button variant="contained" id="btnGoogleAuthentication" color="btn btn-primary" onClick={this.googleAuthenticationClick}>
                    Authenticate With Google
                </Button>
            </div>
        );
    }
}