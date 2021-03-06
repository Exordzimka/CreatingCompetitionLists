import React, {Component} from "react";
import {Tabs, Tab} from "react-bootstrap"
import {Button, Input} from "reactstrap";

export class Database extends Component {
    static displayName = Database.name;

    constructor(props) {
        super(props);
        this.state = {
            directions: this.getDirections(),
            faculties: this.getFaculties(),
            selectedDirection: null,
            selectedFaculty: null,
            directionHidden: true,
            facultyHidden: true,
            facultyId: 0,
            facultyTitle: "",
            directionId: 0,
            directionTitle: "",
            directionShortTitle: "",
            directionCountPlaces: "",
            directionFacultyId: 0
        };
    }

    getDirectionsList() {
        return this.state.directions.map(direction =>
            <option value={direction.id}>
                {direction.shortTitle}
            </option>
        );
    }

    getFacultiesList() {
        return this.state.faculties.map(faculty =>
            <option value={faculty.id}>
                {faculty.title}
            </option>
        );
    }

    async showDirectionFieldsForEdit(e) {
        let selectedDirection = await this.state.directions.filter(direction => direction.id == e.target.value)[0];
        console.log(selectedDirection);
        await this.setState({
            directionHidden: false,
            selectedDirection: selectedDirection,
            directionId: selectedDirection.id,
            directionTitle: selectedDirection.title,
            directionShortTitle: selectedDirection.shortTitle,
            directionCountPlaces: selectedDirection.countForEnrollee,
            directionFacultyId: selectedDirection.facultyId,
        });
        this.setSelectedFaculty();
    } 

    async showDirectionFieldsForCreate() {
        await this.setState({
            directionHidden: false,
            selectedDirection: null,
            directionId: 0,
            directionTitle: "",
            directionShortTitle: "",
            directionCountPlaces: "",
            directionFacultyId: 1,
        });
    }

    setSelectedFaculty() {
        console.log(this.state.selectedDirection);
        document.getElementById("facultiesOfDirections").value = this.state.selectedDirection.facultyId;
    }

    async showFacultyFieldsForEdit(e) {
        let selectedFaculty = this.state.faculties.filter(faculty => faculty.id == e.target.value)[0];
        await this.setState({
            facultyHidden: false,
            selectedFaculty: selectedFaculty,
            facultyId: selectedFaculty.id,
            facultyTitle: selectedFaculty.title,
        });
        await this.setState({
            facultyForAddOrUpdate: {
                id: this.state.facultyId,
                title: this.state.facultyTitle
            }
        });
    }
    
    formDirectionForAddOrUpdate(){
        let data = {};
        data.id = this.state.directionId;
        data.title = this.state.directionTitle;
        data.shortTitle = this.state.directionTitle;
        data.countForEnrollee = this.state.directionCountPlaces;
        data.facultyId = this.state.directionFacultyId;
        return data;
    }
    
    formFacultyForAddOrUpdate(){
        let data = {};
        data.id = this.state.facultyId;
        data.title = this.state.facultyTitle;
        return data;
    }
    
    async showFacultyFieldsForCreate() {
        await this.setState({
            facultyHidden: false,
            selectedFaculty: null,
            facultyId: 0,
            facultyTitle: ""
        });
        await this.setState({
            facultyForAddOrUpdate: {
                id: this.state.facultyId,
                title: this.state.facultyTitle
            }
        });
    }

    getDirections() {
        let xhr = new XMLHttpRequest();
        xhr.open('GET', "http://localhost:80/database/get-directions", false);
        xhr.send();
        return JSON.parse(xhr.responseText);
    }

    getFaculties() {
        let xhr = new XMLHttpRequest();
        xhr.open('GET', "http://localhost:80/database/get-faculties", false);
        xhr.send();
        return JSON.parse(xhr.responseText);
    }

    async handleAddFaculty() {
        let responseStatus;
        await fetch("http://localhost:80/database/add-faculty",{
            method:'POST',
            headers: {'Content-Type': 'application/json'},
            body: JSON.stringify(this.formFacultyForAddOrUpdate())})
            .then(
                function(response) {
                    responseStatus = response.status;
                    console.log(response);
                });
        if(responseStatus===200){
            alert("??????????????????/???????????????? ?????????????????? ??????????????!");
        }
        else{
            alert("?????????????????????? ???????????? ????????????????????!");
        }
        this.setState({
            faculties: this.getFaculties()
        });
    }

    async handleUpdateFaculty() {
        let responseStatus;
        await fetch("http://localhost:80/database/update-faculty",{
            method:'PUT',
            headers: {'Content-Type': 'application/json'},
            body: JSON.stringify(this.formFacultyForAddOrUpdate())})
            .then(
                function(response) {
                    responseStatus = response.status;
                    console.log(response);
                });
        if(responseStatus===200){
            alert("??????????????????/???????????????? ???????????????? ??????????????!");
        }
        else{
            alert("?????????????????????? ???????????? ????????????????????!");
        }
        this.setState({
            faculties: this.getFaculties()
        });
    }

    async handleDeleteFaculty() {
        let responseStatus;
        await fetch("http://localhost:80/database/delete-faculty",{
            method:'DELETE',
            headers: {'Content-Type': 'application/json'},
            body: JSON.stringify(this.state.facultyId)})
            .then(
                function(response) {
                    responseStatus = response.status;
                    console.log(response);
                });
        if(responseStatus===200){
            alert("??????????????????/???????????????? ???????????? ??????????????!");
        }
        else{
            alert("?????????????????????? ???????????? ????????????????!");
        }
        this.setState({
            faculties: this.getFaculties(),
            facultyHidden: true
        });
    }

    async handleAddDirection() {
        let responseStatus;
        await fetch("http://localhost:80/database/add-direction",{
            method:'POST',
            headers: {'Content-Type': 'application/json'},
            body: JSON.stringify(this.formDirectionForAddOrUpdate())})
            .then(
                function(response) {
                    responseStatus = response.status;
                    console.log(response);
                });
        if(responseStatus===200){
            alert("?????????????????????? ?????????????????? ??????????????!");
        }
        else{
            alert("?????????????????????? ???????????? ???????????????????? ??????????????????????!");
        }
        this.setState({
            directions: this.getDirections()
        });
    }

    async handleUpdateDirection() {
        let responseStatus;
        await fetch("http://localhost:80/database/update-direction",{
            method:'PUT',
            headers: {'Content-Type': 'application/json'},
            body: JSON.stringify(this.formDirectionForAddOrUpdate())})
            .then(
                function(response) {
                    responseStatus = response.status;
                    console.log(response);
                });
        if(responseStatus===200){
            alert("?????????????????????? ?????????????????? ??????????????!");
        }
        else{
            alert("?????????????????????? ???????????? ???????????????????? ??????????????????????!");
        }
        this.setState({
            directions: this.getDirections()
        });
    }

    async handleDeleteDirection() {
        let responseStatus;
        await fetch("http://localhost:80/database/delete-direction",{
            method:'DELETE',
            headers: {'Content-Type': 'application/json'},
            body: JSON.stringify(this.state.directionId)})
            .then(
                function(response) {
                    responseStatus = response.status;
                    console.log(response);
                });
        if(responseStatus===200){
            alert("?????????????????????? ?????????????? ??????????????!");
        }
        else{
            alert("?????????????????????? ???????????? ???????????????? ??????????????????????!");
        }
        this.setState({
            directions: this.getDirections(),
            directionHidden: true
        });
    }

    handleChangeSelectFacultyForDirection(e) {
        this.setState({
            directionFacultyId: e.target.value
        });
    }

    render() {
        let directions = this.getDirectionsList();
        let faculties = this.getFacultiesList();
        return (
            <div id={"main"}>
                <Tabs defaultActiveKey="faculties" id="uncontrolled-tab-example" className={"justify-content-center"}>
                    <Tab eventKey="faculties" title="????????????????????/??????????????????">
                        <div style={{display: "table-row"}}>
                            <h3>????????????????????/??????????????????</h3>
                            <hr/>
                        </div>
                        <div style={{display: "table-cell"}}>
                            <div>
                                <Button onClick={() => this.showFacultyFieldsForCreate()} className={"btn btn-primary"}>????????????????
                                    ??????????????????/????????????????</Button>
                            </div>
                            <select onChange={(e) => this.showFacultyFieldsForEdit(e)} id={"facultySelect"} size={22}
                                    style={{width: "240px", marginTop: "5px"}}>
                                {faculties}
                            </select>
                        </div>
                        <div style={{display: "table-cell"}} hidden={this.state.facultyHidden}>
                            <label htmlFor="title">???????????????? ????????????????????/??????????????????</label>
                            <Input id={"title"} type={"text"} value={this.state.facultyTitle} onChange={(e) => this.setState({facultyTitle: e.target.value})}/>
                            <Button style={{marginTop: "10px"}}
                                    onClick={() => !this.state.selectedFaculty ? this.handleAddFaculty() : this.handleUpdateFaculty()}
                                    className={"btn btn-primary"}>{!this.state.selectedFaculty ? "????????????????" : "????????????????"}</Button>
                            <Button style={{marginTop: "10px", marginLeft: "10px"}}
                                    onClick={() => this.handleDeleteFaculty()}
                                    className={"btn btn-danger"} hidden={!this.state.selectedFaculty}>??????????????</Button>
                        </div>
                    </Tab>
                    <Tab eventKey="directions" title="??????????????????????">
                        <div style={{display: "table-row"}}>
                            <h3>??????????????????????</h3>
                            <hr style={{width: "300px"}}/>
                        </div>
                        <div style={{display: "table-cell"}}>
                            <div>
                                <Button onClick={() => this.showDirectionFieldsForCreate()} style={{width: "240px"}}
                                        className={"btn btn-primary"}>???????????????? ??????????????????????</Button>
                            </div>
                            <select onChange={(e) => this.showDirectionFieldsForEdit(e)} id={"directionSelect"} size={22}
                                    style={{width: "240px", marginTop: "5px"}}>
                                {directions}
                            </select>
                        </div>
                        <div style={{display: "table-cell"}}>
                            <div style={{display: "table-cell"}} hidden={this.state.directionHidden}>
                                <label htmlFor="title">???????????????? ??????????????????????</label>
                                <Input id={"title"} type={"text"} value={this.state.directionTitle}
                                       onChange={(e) => this.setState({directionTitle: e.target.value})}/>
                                <label htmlFor="shortTitle">?????????????????????? ????????????????</label>
                                <Input id={"shortTitle"} type={"text"}
                                       onChange={(e) => this.setState({directionShortTitle: e.target.value})}
                                       value={this.state.directionShortTitle}/>
                                <Button style={{marginTop: "10px"}}
                                        onClick={() => !this.state.selectedDirection ? this.handleAddDirection() : this.handleUpdateDirection()}
                                        className={"btn btn-primary"}>{!this.state.selectedDirection ? "????????????????" : "????????????????"}</Button>
                                <Button style={{marginTop: "10px", marginLeft: "10px"}}
                                        className={"btn btn-danger"} onClick={() => this.handleDeleteDirection()}
                                        hidden={!this.state.selectedDirection}>??????????????</Button>
                            </div>
                            <div style={{display: "table-cell", paddingLeft: "5%"}} hidden={this.state.directionHidden}>
                                <label htmlFor="countPlaces">??????</label>
                                <Input id={"countPlaces"} type={"text"}
                                       onChange={(e) => this.setState({directionCountPlaces: e.target.value})}
                                       value={this.state.directionCountPlaces}/>
                                <label htmlFor="facultiesOfDirections">??????????????????/????????????????</label>
                                <div>
                                    <select style={{height: "38px"}} id={"facultiesOfDirections"}
                                            onChange={(e) => this.handleChangeSelectFacultyForDirection(e)}>{faculties}</select>
                                </div>
                            </div>
                        </div>
                    </Tab>
                </Tabs>
            </div>

        );
    }
}