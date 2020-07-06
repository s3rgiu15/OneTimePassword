import React from 'react';
import { Link } from 'react-router-dom';

class HomePage extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            user: {}
        };
    }

    componentDidMount() {
        this.setState({ 
            user: JSON.parse(localStorage.getItem('user'))
        });
    }

    render() {
        const { user } = this.state;
        return (
            <div className="col-lg-12 col-lg-offset-0 col-md-6 col-md-offset-3">
                <h1>Hi {user.firstName}!</h1>
                <p>Congratulations, you are logged in as {user.email}</p>
                <p>
                    <Link to="/login">Logout</Link>
                </p>
            </div>
        );
    }
}

export { HomePage };