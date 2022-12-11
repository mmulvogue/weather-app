import React, { Component } from 'react';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { faLinkedin } from '@fortawesome/free-brands-svg-icons'
import Container from 'react-bootstrap/Container';

export default class AppFooter extends Component {
    render() {
        return (
            <footer class="py-3 my-4 border-top">
                <Container>
                    <p class="text-muted">
                        <a href="https://www.linkedin.com/in/michael-mulvogue-32675339/" target="_blank" class="text-decoration-none text-muted">
                            <FontAwesomeIcon icon={faLinkedin} /> Michael Mulvogue
                        </a>
                    </p>
                </Container>
            </footer>
        );
    }
}