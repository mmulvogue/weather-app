import { render, screen, fireEvent } from '@testing-library/react';
import WeatherServiceForm from './WeatherServiceForm';

test('shows validation error when city is empty', () => {
    render(<WeatherServiceForm />);

    const submitButton = screen.getByRole('button');
    const cityInput = screen.getByLabelText('City name');
    const countryInput = screen.getByLabelText('Country name');

    fireEvent.change(cityInput, { target: { value: '' } });
    fireEvent.change(countryInput, { target: { value: 'NotEmpty' } });

    fireEvent.click(submitButton);

    expect(cityInput).toHaveClass('is-invalid');
    expect(screen.getByText('Please enter a city name')).toBeVisible();
});

test('shows validation error when country is empty', () => {
    render(<WeatherServiceForm />);

    const submitButton = screen.getByRole('button');
    const cityInput = screen.getByLabelText('City name');
    const countryInput = screen.getByLabelText('Country name');

    fireEvent.change(cityInput, { target: { value: 'NotEmpty' } });
    fireEvent.change(countryInput, { target: { value: '' } });

    fireEvent.click(submitButton);
    
    expect(countryInput).toHaveClass('is-invalid');
    expect(screen.getByText('Please enter a country name')).toBeVisible();
});
