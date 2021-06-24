import React, {Component} from 'react';
import {Button, Input, Table} from "reactstrap";
import Modal from 'react-modal'

export class Home extends Component {
    static displayName = Home.name;

    constructor(props) {
        super(props);

        this.getFiles = this.getFiles.bind(this);

        this.state = {
            searchResponse:  this.getFiles(null),
            modalVisible: false
        };
        console.log(this.state.searchResponse);
        Modal.setAppElement(document.getElementById('main'));
    }

    showModal(){
        this.setState({
            modalVisible: true
        });
    }

    hideModal(){
        this.setState({
            modalVisible: false
        });
    }

    renderTables(searchResponse) {
        if(searchResponse!=null)
            return (
                <tbody id={"tbody"}>
                {searchResponse.files.map(file =>
                    <tr>
                        <td align={"center"}>
                            <a href="asd" >{file.name}</a>
                        </td>
                    </tr>
                )}
                </tbody>
            )
    }

    renderNextButton() {

        return (
            <Button className={"btn btn-primary"}
                    onClick={() => this.nextButtonClick()}
                    style={{float: "right"}}>
                Следующая страница
            </Button>
        )
    }
    nextButtonClick(){
        this.setState({
            searchResult: this.getFiles(this.state.searchResponse.nextPageToken)
        });
        this.render();
    }
    render() {
        let content = this.renderTables(this.state.searchResponse);
        return (
            <div id={"main"}>
                <Button onClick={() => this.showModal()}>Создать таблицу</Button>
                <Modal isOpen={this.state.modalVisible} aria={{
                    labelledby: "dialogheader",
                    describedby: "full_description"
                }}>
                    <div>
                        <h1 id="heading" style={{display: "block"}}>Создание новой таблицы</h1>
                        <Button className={"btn btn-danger"} onClick={() => this.hideModal() } style={{display: "block", position: "absolute", top: "10px", right: "10px"}}>Закрыть</Button>
                    </div>
                    <div>
                        <Input type={"text"} style={{width: "400px", marginBottom:"3px"}} placeholder={"Название таблицы"}/>
                    </div>


                </Modal>
                <Table striped condensed hover>
                    <thead>
                    <tr>
                        <th align={"center"}>
                            <h1>Название файла</h1>
                        </th>
                    </tr>
                    </thead>
                    {content}
                </Table>
                <div className={"btn-group"}>
                    <Button onClick={() => this.setState({
                        searchResult: this.getFiles(null)
                    })}>Предыдущая страница</Button>
                    {this.renderNextButton()}
                </div>

            </div>
        );
    }

    async checkSpreadSheets() {
        alert("asasa");
        // let response = await fetch("https://localhost:5001/spreadsheets/fill");
        let response = await fetch("https://localhost:5001/drive/search");
    }

    getSheets(tokenPage) {
        let result;
        if (tokenPage == null){

            fetch("https://localhost:5001/drive/search").then(response =>{
                response.json().then(body => {
                    console.log(body);
                    result = body;
                });
            });
            return result;
        }

        fetch("https://localhost:5001/drive/search?token-page=" + tokenPage).then(response =>{
            response.json().then(body => {
                console.log(body);
                result = body;
            });
        });

    }

    getFiles(tokenPage){
        if(this.isAuthenticated()){
            let xhr = new XMLHttpRequest();
            if(tokenPage == null)
                xhr.open('GET', "https://localhost:5001/drive/search", false);
            else
                xhr.open('GET', "https://localhost:5001/drive/search?token-page=" + tokenPage, false);
            xhr.send();
            return JSON.parse(xhr.responseText);
        }
    }

    isAuthenticated(){
        let xhr = new XMLHttpRequest();
        xhr.open('GET', "https://localhost:5001/user/is-authenticated", false);
        xhr.send();
        return xhr.responseText !== "not authenticated";
    }
}
