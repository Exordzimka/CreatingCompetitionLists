import React, {Component} from 'react';
import {Collapse, Container, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink} from 'reactstrap';
import {Link} from 'react-router-dom';
import './NavMenu.css';

export class NavMenu extends Component {
    static displayName = NavMenu.name;

    constructor(props) {
        super(props);

        this.toggleNavbar = this.toggleNavbar.bind(this);
        this.state = {
            collapsed: true,
        };
    }

    componentDidMount() {
        this.checkAuthenticate();
    }

    toggleNavbar() {
        this.setState({
            collapsed: !this.state.collapsed
        });
    }

    async checkAuthenticate() {
        let response = await fetch("https://localhost:5001/user/is-authenticated");
        let isAuthenticated = await response.text();
        this.setState(
            {
                authenticated: isAuthenticated
            }
        )
    }

    formGreetings(authenticated) {
        let greetingsValue = <p class="text-danger font-weight-bold">Вы не аутентифицированы</p>;
        if (authenticated)
            greetingsValue = <p class="text-success font-weight-bold">{this.state.authenticated}</p>;
        return <NavItem>
            <NavLink tag={NavItem}>{greetingsValue}</NavLink>
        </NavItem>;
    }

    formSignInOrSignOut(authenticated) {
        let link = "authentication/google-login";
        let linkText = "Login";
        if (authenticated)
        {
            link = "authentication/google-logout";
            linkText = "Logout";
        }
        return <NavItem>
            <NavLink className="text-primary" href={link}>{linkText}</NavLink>
        </NavItem>;
    }

    render() {
        let boolAuthenticated = this.state.authenticated !== "not authenticated";
        let greetings = this.formGreetings(boolAuthenticated);
        let authentication = this.formSignInOrSignOut(boolAuthenticated);
        return (
            <header>
                <Navbar className="navbar-expand-sm navbar-toggleable-sm ng-white border-bottom box-shadow mb-3" light>
                    <Container>
                        <NavbarBrand tag={Link} to="/">CreatingCompetitionLists</NavbarBrand>
                        <NavbarToggler onClick={this.toggleNavbar} className="mr-2"/>
                        <Collapse className="d-sm-inline-flex flex-sm-row-reverse" isOpen={!this.state.collapsed}
                                  navbar>
                            <ul className="navbar-nav flex-grow">
                                <NavItem>
                                    <NavLink tag={Link} className="text-dark" data-toggel to="/">Home</NavLink>
                                </NavItem>
                                <NavItem>
                                    <NavLink tag={Link} className="text-dark" to="/counter">Counter</NavLink>
                                </NavItem>
                                <NavItem>
                                    <NavLink tag={Link} className="text-dark" to="/fetch-data">Fetch data</NavLink>
                                </NavItem>
                                {greetings}
                                {authentication}
                            </ul>
                        </Collapse>
                    </Container>
                </Navbar>
            </header>
        );
    }
}
