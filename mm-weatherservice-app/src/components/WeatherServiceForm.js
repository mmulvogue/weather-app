import React, { Component } from 'react';
import Button from 'react-bootstrap/Button';
import Form from 'react-bootstrap/Form';
import Alert from 'react-bootstrap/Alert';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { faSpinner } from '@fortawesome/free-solid-svg-icons'

const WeatherResultTypes = {
    Success: 0,
    NotFound: 1,
    Error: 2
}

export default class WeatherServiceForm extends Component {
    constructor(props) {
        super(props);

        this.state = {
            cityName: 'Melbourne',
            countryName: 'AUS',
            validationErrors: {},
            validated: false,
            processing: false,
            showWeather: false,
            weatherResult: null,
            weatherDescription: ''
        };

        this.handleFormInputChange = this.handleFormInputChange.bind(this);
        this.handleFormSubmit = this.handleFormSubmit.bind(this);
    }

    validateInput(target) {
        if (target.required !== 'undefined') {
            let validationErrors = this.state.validationErrors
            validationErrors[target.id] = !target.value;
            this.setState({ validationErrors });
        }
    }

    handleFormInputChange(event) {
        this.setState({ [event.target.id]: event.target.value }, () => { this.validateInput(event.target); });
    }

    async handleFormSubmit(event) {
        event.preventDefault();
        event.stopPropagation();

        const form = event.currentTarget;
        if (form.checkValidity() !== false) {
            this.setState({
                processing: true,
                showWeatherError: false
            });
            try {
                const response = await fetch('weather?' + new URLSearchParams({
                    city: this.state.cityName,
                    country: this.state.countryName,
                }));

                console.log(response);
                if (response.status === 200) {
                    const weatherData = await response.json();
                    this.setState({
                        weatherDescription: weatherData.description,
                        weatherResult: WeatherResultTypes.Success
                    });
                }
                else if (response.status === 404) {
                    this.setState({
                        weatherDescription: '',
                        weatherResult: WeatherResultTypes.NotFound
                    });
                }
                else {
                    this.setState({
                        weatherDescription: '',
                        weatherResult: WeatherResultTypes.Error
                    });
                }

            } finally {
                this.setState({
                    showWeather: true,
                    processing: false
                });
            }
        }
        this.setState({ validated: true });
    }

    renderWeatherResult() {        
        if (this.state.weatherResult === WeatherResultTypes.Success) {
            return (
                <Alert variant="success" className="mt-3">
                    <p className="mb-0">The current weather for <em> {this.state.cityName}, {this.state.countryName}</em> is: <strong>{this.state.weatherDescription}</strong></p>
                </Alert>
            );
        }
        else if (this.state.weatherResult === WeatherResultTypes.NotFound) {
            return (
                <Alert variant="warning" className="mt-3">
                    <p>No weather was found for <em> {this.state.cityName}, {this.state.countryName}</em></p>
                    <p className="mb-0">Check the city and country are correct and try again.</p>
                </Alert>
            );
        }
        else if (this.state.weatherResult === WeatherResultTypes.Error) {
            return (
                <Alert variant="danger" className="mt-3">
                    <p className="mb-0">Sorry, an error occured retrieving weather for <em> {this.state.cityName}, {this.state.countryName}</em></p>
                </Alert>
            );
        }
    }

    render() {
        return (
            <div>
                <Form noValidate onSubmit={this.handleFormSubmit}>
                    <fieldset disabled={this.state.processing}>
                        <legend>Get weather</legend>
                        <Form.Group className="mb-3" controlId="cityName">
                            <Form.Label>City name</Form.Label>
                            <Form.Control
                                required
                                type="text"
                                placeholder="Enter city name"
                                value={this.state.cityName}
                                onChange={this.handleFormInputChange}
                                isInvalid={this.state.validated && this.state.validationErrors.cityName}
                            />
                            <Form.Control.Feedback type="invalid">
                                Please enter a city name
                            </Form.Control.Feedback>
                        </Form.Group>

                        <Form.Group className="mb-3" controlId="countryName">
                            <Form.Label>Country name</Form.Label>
                            <Form.Control
                                required
                                type="text"
                                placeholder="Enter country name"
                                value={this.state.countryName}
                                onChange={this.handleFormInputChange}
                                isInvalid={this.state.validated && this.state.validationErrors.countryName}
                            />
                            <Form.Control.Feedback type="invalid">
                                Please enter a country name
                            </Form.Control.Feedback>
                        </Form.Group>
                        <Button variant="primary" type="submit">
                            Submit
                        </Button>
                        {this.state.processing && <span className="ps-1"><FontAwesomeIcon icon={faSpinner} spin /></span>}
                    </fieldset>
                </Form>
                { 
                    this.state.showWeather ? this.renderWeatherResult() : null                   
                }
            </div>
        );
    }
}