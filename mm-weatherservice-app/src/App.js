import React, { Component } from 'react';
import Container from 'react-bootstrap/Container';
import WeatherServiceForm from './components/WeatherServiceForm';

export default class App extends Component {
    static displayName = App.name;

    constructor(props) {
        super(props);
    }
    
    render() {     
        return (
            <Container>
                <WeatherServiceForm />
            </Container>
        );
    }
}
