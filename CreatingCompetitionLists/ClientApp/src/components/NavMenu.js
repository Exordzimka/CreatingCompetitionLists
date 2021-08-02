import React, {Component} from 'react';
import {Collapse, Container, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink} from 'reactstrap';
import {Link} from 'react-router-dom';
import './NavMenu.css';

export class NavMenu extends Component {
    static displayName = NavMenu.name;

    constructor(props) {
        super(props);

        this.toggleNavbar = this.toggleNavbar.bind(this);
        this.checkAuthenticate();
        this.state = {
            collapsed: true,
        };
    }

    componentDidMount() {
        // this.checkAuthenticate();
    }

    toggleNavbar() {
        this.setState({
            collapsed: !this.state.collapsed
        });
    }

    async checkAuthenticate() {
        let response = await fetch("http://localhost:80/user/is-authenticated");
        let isAuthenticated = await response.text();
        console.log("IS AUTH" + isAuthenticated);
        this.setState(
            {
                authenticated: isAuthenticated
            }
        )
    }

    formGreetings(authenticated) {
        let greetingsValue = <p class="text-danger font-weight-bold">Вы не аутентифицированы</p>;
        if (authenticated)
            greetingsValue = <div><span className={"text-dark"}>Учетная запись: </span><span className="text-dark font-italic">{this.state.authenticated}</span></div>;
        return <NavItem>
            <NavLink tag={NavItem}>{greetingsValue}</NavLink>
        </NavItem>;
    }

    formSignInOrSignOut(authenticated) {
        let link = "authentication/google-login";
        let linkText = "Войти";
        if (authenticated)
        {
            link = "authentication/google-logout";
            linkText = "Выйти";
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
                        <NavbarBrand style={{backgroundImage: 'url(/logo.png)'}} tag={Link} to="/">Создание автоматизированных конкурсных списков</NavbarBrand>
                        <NavbarToggler onClick={this.toggleNavbar} className="mr-2"/>
                        <Collapse className="d-sm-inline-flex flex-sm-row-reverse" isOpen={!this.state.collapsed}
                                  navbar>
                            <ul className="navbar-nav flex-grow">
                                <NavItem>
                                    <NavLink tag={Link} className={"text-primary"} to={"/database"}>База Данных</NavLink>
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
