import React from 'react';
import ReactDOM from 'react-dom/client';
import './index.scss';
import App from './App';
import AppHeader from './components/AppHeader';
import AppFooter from './components/AppFooter';
import reportWebVitals from './reportWebVitals';

const root = ReactDOM.createRoot(document.getElementById('root'));
root.render(
    <React.StrictMode>
        <div className="cover-container d-flex w-100 h-100 mx-auto flex-column" >            
            <AppHeader />            
            <main className="flex-shrink-0 pt-4">
                <App />
            </main>
            <div className="mt-auto">
                <AppFooter />
            </div>            
        </div>
    </React.StrictMode>
);

// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
reportWebVitals();
