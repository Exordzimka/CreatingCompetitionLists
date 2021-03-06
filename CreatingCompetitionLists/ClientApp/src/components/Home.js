import React, {Component} from 'react';
import {Button, Input, Table} from "reactstrap";
import Modal from 'react-modal'
import {Checkbox, Radio} from "semantic-ui-react";
import CustomizedDialogs from "./CutomDialog";
import Dialog from "@material-ui/core/Dialog";
import Typography from "@material-ui/core/Typography";
import {withStyles} from "@material-ui/core/styles";
import MuiDialogTitle from "@material-ui/core/DialogTitle";
import IconButton from "@material-ui/core/IconButton";
import CloseIcon from "@material-ui/icons/Close";
import MuiDialogContent from "@material-ui/core/DialogContent";
import MuiDialogActions from "@material-ui/core/DialogActions";
import {PanoramaWideAngle} from "@material-ui/icons";

const styles = (theme) => ({
    root: {
        margin: 0,
        padding: theme.spacing(2),
    },
    closeButton: {
        position: 'absolute',
        right: theme.spacing(1),
        top: theme.spacing(1),
        color: theme.palette.grey[500],
    },
});

const DialogTitle = withStyles(styles)((props) => {
    const {children, classes, onClose, ...other} = props;
    return (
        <MuiDialogTitle disableTypography className={classes.root} {...other}>
            <Typography variant="h6">{children}</Typography>
            {onClose ? (
                <IconButton aria-label="close" className={classes.closeButton} onClick={onClose}>
                    <CloseIcon/>
                </IconButton>
            ) : null}
        </MuiDialogTitle>
    );
});

const DialogContent = withStyles((theme) => ({
    root: {
        padding: theme.spacing(2),
    },
}))(MuiDialogContent);

const DialogActions = withStyles((theme) => ({
    root: {
        margin: 0,
        padding: theme.spacing(1),
    },
}))(MuiDialogActions);

export class Home extends Component {
    static displayName = Home.name;

    constructor(props) {
        super(props);
        this.getFiles = this.getFiles.bind(this);
        this.reduceDirections = this.reduceDirections.bind(this);
        this.state = {
            searchResponse: null,
            createVisible: false,
            dataForCreating: null,
            dropdownOpen: false,
            checkBoxes: null,
            editVisible: false,
            chosenTable: null,
            previousButtonDisable: false,
            nextButtonDisable: false,
            authenticated: false
        };

        Modal.setAppElement(document.getElementById('main'));
    }

    async componentDidMount() {
        await this.isAuthenticated();
        await this.getFiles();
        await this.setState({
            previousButtonDisable: this.state.searchResponse?.previousPageToken == null,
            nextButtonDisable: this.state.searchResponse?.nextPageToken == null,
        })
    }

    updateDialog() {
        return (
            <div>
                <Dialog onClose={() => this.hideEdit()} aria-labelledby="customized-dialog-title"
                        open={this.state.editVisible}>
                    <DialogTitle id="customized-dialog-title" onClose={() => this.hideEdit()}>
                        {this.renderTableName()}
                    </DialogTitle>
                    <DialogContent dividers>
                        <Typography gutterBottom>
                            <h2>????????????????:</h2>
                            ?????? ?????????????? ???? ???????????? "????????????????" ?????????????????? ???????????????????? ???????????? ??????????????????.
                        </Typography>
                    </DialogContent>
                    <DialogActions className={"justify-content-center"}>
                        <Button autoFocus onClick={() => this.updateSpreadsheet(this.state.chosenTable.id)}
                                color="primary">
                            ????????????????
                        </Button>
                        <Button onClick={() => this.reduceDirections(this.state.chosenTable.id)} color="primary">
                            ?????????????????? ???????????????? ??????????????????????
                        </Button>
                    </DialogActions>
                </Dialog>
            </div>
        );
    }

    showCreate() {
        let dataResponse = this.getData();
        this.setState({
            createVisible: true,
            dateForCreating: dataResponse
        });
        this.getCheckBoxes(dataResponse, dataResponse.faculties[0].id);
    }

    hideCreate() {
        this.setState({
            createVisible: false,
            dateForCreating: null
        });
    }


    showEdit(file) {
        this.setState({
            editVisible: true,
            chosenTable: file
        });
    }

    hideEdit() {
        this.setState({
            editVisible: false
        });
    }

    renderTables(searchResponse) {
        console.log(searchResponse);
        if (searchResponse != null)
            return (
                <tbody id={"tbody"}>
                {searchResponse.files.map(file =>
                    <tr align={"center"}>
                        <Button style={{width: "100%"}} className={"btn btn-light"}
                                onClick={() => this.showEdit(file)}>{file.name}</Button>
                    </tr>
                )}
                </tbody>
            )
    }

    renderPreviousButton() {
        return (
            <Button className={"btn btn-primary"}
                    onClick={() => this.previousButtonClick()}
                    style={{marginRight: "5px"}}>
                ???????????????????? ????????????????
            </Button>
        )
    }

    renderNextButton() {
        return (
            <Button className={"btn btn-primary"}
                    onClick={() => this.nextButtonClick()}
                    style={{marginRight: "5px"}}>
                ?????????????????? ????????????????
            </Button>
        )
    }

    async nextButtonClick() {
        await this.getFiles(true, false);
        let response = this.state.searchResponse;
        await this.setState({
            nextButtonDisable: response.nextPageToken == null,
            previousButtonDisable: response.previousPageToken == null,
        });
        console.log(this.state.searchResponse);
    }

    async previousButtonClick() {
        await this.getFiles(false, true);
        let response = this.state.searchResponse;
        await this.setState({
            nextButtonDisable: response.nextPageToken == null,
            previousButtonDisable: response.previousPageToken == null,
        });
    }

    getOptions() {
        if (this.state.dateForCreating == null) return;
        return this.state.dateForCreating.faculties.map(faculty =>
            <option value={faculty.id}>
                {faculty.title}
            </option>
        );
    }

    getCheckBoxes(dataForCreating, selectedFaculty) {
        if (dataForCreating == null) return;
        this.setState({
            checkBoxes: null
        });
        let faculty;
        for (let i = 0; i < dataForCreating.faculties.length; i++) {
            let currentFaculty = dataForCreating.faculties[i];
            if (currentFaculty.id == selectedFaculty) {
                faculty = currentFaculty;
            }
        }
        if (faculty.directions != null && faculty.directions.length > 0) {
            this.setState({
                checkBoxes: faculty.directions.map(direction =>
                    <Checkbox value={direction.id} name={"directions"} label={direction.title}/>)
            });
        }
    }

    renderTableName() {
        if (this.state.chosenTable == null) return;
        return (
            <div><h2>???????????????????? ?????????????????????? ???????????? </h2><a href={this.state.chosenTable.webViewLink} target="_blank"><h2
                id="editHeading" style={{display: "block"}}>{this.state.chosenTable.name}</h2></a></div>);
    }

    render() {
        // let content = this.renderTables(this.state.searchResponse);
        let options = this.getOptions();

        return (
            <div id={"main"}>
                <Button className={"btn btn-primary"} onClick={() => this.showCreate()} style={{marginBottom: "5px"}}>??????????????
                    ??????????????</Button>
                <Modal width={"50px"} isOpen={this.state.createVisible} aria={{
                    labelledby: "dialogheader",
                    describedby: "full_description"
                }}>
                    <div style={{backgroundImage: 'url(/mupoch.jpg)'}}>
                        <h1 id="heading" style={{display: "block"}}>???????????????? ???????????????????????????????????? ???????????????????? ??????????????
                            ????????????????????????</h1>
                        <Button className={"btn btn-danger"} onClick={() => this.hideCreate()} style={{
                            display: "block",
                            position: "absolute",
                            bottom: "10px",
                            right: "10px"
                        }}>??????????????</Button>
                    </div>
                    <div>
                        <Input id={"tableName"} type={"text"} style={{width: "400px", marginBottom: "5px"}}
                               placeholder={"???????????????? ??????????????"}/>
                        <div style={{float: "left"}}>
                            <h3>??????????????????/????????????????</h3>
                            <select id={"select"}
                                    onChange={() => this.getCheckBoxes(this.state.dateForCreating, document.getElementById("select").value)}>
                                {options}
                            </select>
                            <h3>??????????????????????</h3>
                            <div id={"checkBoxes"}>
                                {this.state.checkBoxes}
                            </div>
                            <Button className={"btn btn-primary"}
                                    onClick={() => this.createSpreadsheets()}>??????????????</Button>
                        </div>
                        <div style={{float: "right"}}>
                            <h3>???????????????????? ???????????? ????????????</h3>
                            {/*<Radio checked={"checked"} name={"stages"} value={1} label="1"/>*/}
                            {/*<Radio name={"stages"} value={2} label="2"/>*/}
                            <select id={"stage"} name={"stages"}>
                                <option value="1">
                                    1
                                </option>
                                <option value="2">
                                    2
                                </option>
                            </select>
                            <h3>???????????????????? ?????????????????? ??????????????????????</h3>
                            <select id={"countPossibleDirections"}>
                                <option value="1">
                                    1
                                </option>
                                <option value="2">
                                    2
                                </option>
                                <option value="3">
                                    3
                                </option>
                                <option value="4">
                                    4
                                </option>
                                <option value="5">
                                    5
                                </option>
                                <option value="6">
                                    6
                                </option>
                                <option value="7">
                                    7
                                </option>
                                <option value="8">
                                    8
                                </option>
                                <option value="9">
                                    9
                                </option>
                                <option value="10">
                                    10
                                </option>
                            </select>
                        </div>
                    </div>
                </Modal>
                <Table striped condensed hover>
                    <thead>
                    <tr>
                        <th>
                            <h1 align={"center"}>???????????? ???????????? ???? Google Drive</h1>
                        </th>
                    </tr>
                    </thead>
                    {this.renderTables(this.state.searchResponse)}
                </Table>
                {this.updateDialog()}
                <div className={"btn-group"}>
                    <Button className={"btn btn-primary"}
                            disabled={this.state.previousButtonDisable}
                            onClick={() => this.previousButtonClick()}
                            style={{marginBottom: "25px"}}>
                        ???????????????????? ????????????????
                    </Button>
                    <Button className={"btn btn-primary"}
                            disabled={this.state.nextButtonDisable}
                            onClick={() => this.nextButtonClick()}
                            style={{marginLeft: "232%", marginBottom: "25px"}}>
                        ?????????????????? ????????????????
                    </Button>
                </div>
            </div>
        );
    }

    async createSpreadsheets() {
        let directionIds = [];
        let checkBoxes = document.querySelectorAll('input[name="directions"]:checked');
        checkBoxes.forEach((checkBox) => {
            directionIds.push(checkBox.value);
        });
        let data = {};
        data.tableName = document.getElementById("tableName").value;
        data.facultetId = document.getElementById("select").value;
        data.directionIds = directionIds;
        data.stage = document.getElementById("stage").value;
        data.possibleDirections = document.getElementById("countPossibleDirections").value;
        if (data.tableName == null || data.tableName === "") {
            alert("???????????????? ?????????????? ???? ??????????????????");
            return;
        }
        if (directionIds.length === 0) {
            alert("???? ?????????????? ??????????????????????");
            return;
        }
        let responseStatus;
        await fetch("http://localhost:80/spreadsheets/create", {
            method: 'POST',
            headers: {'Content-Type': 'application/json'},
            body: JSON.stringify(data)
        })
            .then(
                function (response) {
                    responseStatus = response.status;
                });
        if (responseStatus === 200)
            alert("?????????????? ?????????????? ??????????????");
        else
            alert("?????????????? ???? ??????????????????!");
    }

    async updateSpreadsheet(spreadsheetId) {
        let responseStatus;
        let responseText;
        let url = "http://localhost:80/spreadsheets/highlight-originals?spreadsheetId=" + spreadsheetId;
        await fetch(url, {
            method: 'POST'
        })
            .then(
                await function (response) {
                    responseStatus = response.status;
                    responseText = response.text().then(function (text) {
                        return text;
                    });
                });
        await responseText.then((text) => responseText = text);
        if (responseStatus === 200) {
            if (responseText === "Wait")
                alert("?????????????????????? ???????????? ??????????????, ???????????????????? ??????????????????!");
            else
                alert("?????????????? ?????????????? ??????????????????");
        } else
            alert("?????????????? ???? ????????????????????!");
    }

    async reduceDirections(spreadsheetId) {
        let responseStatus;
        let responseText;
        let url = "http://localhost:80/spreadsheets/reduce-directions?spreadsheetId=" + spreadsheetId;
        await fetch(url, {
            method: 'POST'
        })
            .then(
                await function (response) {
                    responseStatus = response.status;
                    responseText = response.text().then(function (text) {
                        return text;
                    });
                });
        await responseText.then((text) => responseText = text);
        if (responseStatus === 200) {
            alert("?????????????????????? ??????????????????");
        } else
            alert("?????? ???? ?????????? ???? ??????!");
    }

    async getFiles(next, previous) {
        console.log("GET");
        if (this.state.authenticated) {
            let xhr = new XMLHttpRequest();
            let url;
            if (next) {
                url = "http://localhost:80/drive/search?next=true";
            } else if (previous) {
                url = "http://localhost:80/drive/search?previous=true";
            } else {
                url = "http://localhost:80/drive/search";
            }
            let response = await fetch(url);
            // console.log(await response.json());
            // let response1 = await JSON.parse(response.responseText);
            // console.log("RESPONSE "+response1);
            let responseJson = await response.json();
            this.setState({
                searchResponse: responseJson
            });
        }
    }

    async isAuthenticated() {
        let response = await fetch("http://localhost:80/user/is-authenticated");
        let isAuthenticated = await response.text();
        await this.setState({
            authenticated: isAuthenticated.responseText !== "not authenticated"
        });
    }

    getData() {
        let xhr = new XMLHttpRequest();
        xhr.open('GET', "http://localhost:80/spreadsheets/getData", false);
        xhr.send();
        return JSON.parse(xhr.responseText);
    }
}
