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
            facultyHidden: true
        };
    }

    getDirectionsList() {
        console.log()
        return this.state.directions.map(direction =>
            <option value={direction.id}>
                {direction.shortTitle}
            </option>
        );
    }

    getFacultiesList() {
        console.log()
        return this.state.faculties.map(faculty =>
            <option value={faculty.id}>
                {faculty.title}
            </option>
        );
    }

    render() {
        let directions = this.getDirectionsList();
        let faculties = this.getFacultiesList();
        return (
            <div id={"main"}>
                <Tabs defaultActiveKey="faculties" id="uncontrolled-tab-example" className={"justify-content-center"}>
                    <Tab eventKey="faculties" title="Факультеты/Институты">
                        <div style={{display: "table-row"}}>
                            <h3>Факультеты/Институты</h3>
                            <hr/>
                        </div>
                        <div style={{display: "table-cell"}}>
                            <div>
                                <Button onClick={() => this.showFacultyFieldsForCreate()} className={"btn btn-primary"}>Добавить
                                    факультет/институт</Button>
                            </div>
                            <select onChange={() => this.showFacultyFieldsForEdit()} id={"facultySelect"} size={22}
                                    style={{width: "240px", marginTop: "5px"}}>
                                {faculties}
                            </select>
                        </div>
                        {this.facultyContent()}
                    </Tab>
                    <Tab eventKey="directions" title="Направления">
                        <div style={{display: "table-row"}}>
                            <h3>Направления</h3>
                            <hr style={{width: "300px"}}/>
                        </div>
                        <div style={{display: "table-cell"}}>
                            <div>
                                <Button onClick={() => this.showDirectionFieldsForCreate()} style={{width: "240px"}}
                                        className={"btn btn-primary"}>Добавить направление</Button>
                            </div>
                            <select onChange={() => this.showDirectionFieldsForEdit()} id={"directionSelect"} size={22}
                                    style={{width: "240px", marginTop: "5px"}}>
                                {directions}
                            </select>
                        </div>
                        {this.directionContent(faculties)}
                    </Tab>
                </Tabs>
            </div>

        );
    }

    
    
    async showDirectionFieldsForEdit() {
        let selectedIdOfDirection = document.getElementById("directionSelect").value;
        await this.setState({
            directionHidden: false,
            selectedDirection: this.state.directions.filter(direction => direction.id == selectedIdOfDirection)[0]
        });
        this.setSelectedFaculty();
    }

    async showDirectionFieldsForCreate() {
        await this.setState({
            directionHidden: false,
            selectedDirection: null
        });
    }

    setSelectedFaculty() {
        console.log(this.state.selectedDirection);
        document.getElementById("facultiesOfDirections").value = this.state.selectedDirection.facultyId;
    }
    
    showFacultyFieldsForEdit() {
        let selectedIdOfFaculty = document.getElementById("facultySelect").value;
        this.setState({
            facultyHidden: false,
            selectedFaculty: this.state.faculties.filter(faculty => faculty.id == selectedIdOfFaculty)[0]
        });
    }

    showFacultyFieldsForCreate() {
        this.setState({
            facultyHidden: false,
            selectedFaculty: null
        });
    }

    getDirections() {
        let xhr = new XMLHttpRequest();
        xhr.open('GET', "https://localhost:5001/database/get-directions", false);
        xhr.send();
        return JSON.parse(xhr.responseText);
    }

    getFaculties() {
        let xhr = new XMLHttpRequest();
        xhr.open('GET', "https://localhost:5001/database/get-faculties", false);
        xhr.send();
        return JSON.parse(xhr.responseText);
    }
    
    facultyContent(){
        if(this.state.selectedFaculty!=null){
            return(
                <div style={{display: "table-cell"}} hidden={this.state.facultyHidden}>
                    <label htmlFor="title">Название факультета/института</label>
                    <Input id={"title"} type={"text"} value={this.state.selectedFaculty.title}/>
                    <Button style={{marginTop: "10px"}} className={"btn btn-primary"}>Обновить</Button>
                </div>
            );
        }
        else {
            return(
                <div style={{display: "table-cell"}} hidden={this.state.facultyHidden}>
                    <label htmlFor="title">Название факультета/института</label>
                    <Input id={"title"} type={"text"} value={""}/>
                    <Button style={{marginTop: "10px"}} className={"btn btn-primary"}>Добавить</Button>
                </div>
            );
        }
    }
    
    directionContent(faculties){
        if(this.state.selectedDirection!=null){
            return (<div style={{display: "table-cell"}}>
                <div style={{display: "table-cell"}} hidden={this.state.directionHidden}>
                    <label htmlFor="title">Название направления</label>
                    <Input id={"title"} type={"text"} value={this.state.selectedDirection.title}/>
                    <label htmlFor="shortTitle">Сокращенное название</label>
                    <Input id={"shortTitle"} type={"text"} value={this.state.selectedDirection.shortTitle}/>
                    <Button style={{marginTop: "10px"}} className={"btn btn-primary"}>Обновить</Button>
                </div>
                <div style={{display: "table-cell", paddingLeft: "5%"}} hidden={this.state.directionHidden}>
                    <label htmlFor="countPlaces">КЦП</label>
                    <Input id={"countPlaces"} type={"text"} value={this.state.selectedDirection.countForEnrollee}/>
                    <label htmlFor="facultiesOfDirections">Факультет/Институт</label>
                    <div>
                        <select style={{height: "38px"}} id={"facultiesOfDirections"}>{faculties}</select>
                    </div>
                </div>
            </div>);
        }
        else
        {
            return (<div style={{display: "table-cell"}}>
                <div style={{display: "table-cell"}} hidden={this.state.directionHidden}>
                    <label htmlFor="title">Название направления</label>
                    <Input id={"title"} type={"text"} value={""}/>
                    <label htmlFor="shortTitle">Сокращенное название</label>
                    <Input id={"shortTitle"} type={"text"} value={""}/>
                    <Button style={{marginTop: "10px"}} className={"btn btn-primary"}>Добавить</Button>
                </div>
                <div style={{display: "table-cell", paddingLeft: "5%"}} hidden={this.state.directionHidden}>
                    <label htmlFor="countPlaces">КЦП</label>
                    <Input id={"countPlaces"} type={"text"} value={""}/>
                    <label htmlFor="facultiesOfDirections">Факультет/Институт</label>
                    <div>
                        <select style={{height: "38px"}} id={"facultiesOfDirections"}>{faculties}</select>
                    </div>
                </div>
            </div>);
        }
    }
}