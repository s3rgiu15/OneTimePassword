import React from 'react';

import { userService } from '../_services';

class TwoFactorAuthPage extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            username: '',
            password: '',
            submitted: false,
            loading: false,
            error: '',
            user: { userId: ''},
            inputToken: '',
            seconds: 0
        };

        this.handleChange = this.handleChange.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
    }

    componentDidMount() {
        var user = JSON.parse(localStorage.getItem('user'));
        this.setState({ 
            user: user,
            seconds: user.seconds
        });

        this.countdown()
    }

    countdown() {
        this.myInterval = setInterval(() => {
            const { seconds } = this.state

            if (seconds > 0) {
                this.setState(({ seconds }) => ({
                    seconds: seconds - 1
                }))
            }
            if (seconds === 0) {
                clearInterval(this.myInterval)
            } 
        }, 1000)
    }

    componentWillUnmount() {
        clearInterval(this.myInterval)
    }          

    handleChange(e) {
        const { name, value } = e.target;
        this.setState({ [name]: value });
    }

    regenerateToken(userId) {
        //this.setState({ loading: true });
        userService.regenerateToken(userId)
            .then(
                user => {
                    console.log(user)
                    if (user.error == "") {
                        this.setState( { seconds: user.seconds, inputToken : user.token, user: user });
                        this.countdown();
                        //const { from } = this.props.location.state || { from: { pathname: "/twoFactorAuth" } };
                        //this.props.history.push(from);
                    } else {
                        this.setState({ error: user.error, loading: false })
                    }
                },
                error => this.setState({ loading: false })
            );
    }

    handleSubmit(e) {
        e.preventDefault();

        this.setState({ submitted: true });
        const { inputToken, user } = this.state;

        // stop here if form is invalid
        if (!inputToken) {
            return;
        }

        this.setState({ loading: true });

        userService.validateToken(user.userId, inputToken)
            .then(
                user => {
                    if(user.error == "") {
                        const { from } = this.props.location.state || { from: { pathname: "/" } };
                        this.props.history.push(from);
                    } else {
                        this.setState({ error: user.error, loading: false })
                    }
                },
                error => this.setState({ error, loading: false })
            );
    }

    render() {
        const { submitted, loading, error, user, inputToken, seconds } = this.state;

        return (
            <div className="col-lg-12 col-lg-offset-0 col-md-6 col-md-offset-3">
                <div className="alert alert-info">
                    UserId: {user.userId}<br />
                    Token: {user.token}
                </div>
                <h2>Two Factor Auth</h2>
                <form name="form" onSubmit={this.handleSubmit}>
                    <div className={'form-group' + (submitted && !user.userId ? ' has-error' : '')}>
                        <label htmlFor="UserId">UserId</label>
                        <input type="text" className="form-control" name="UserId" value={user.userId} disabled />
                    </div>
                    <div className={'form-group' + (submitted && !inputToken ? ' has-error' : '')}>
                        <label htmlFor="inputToken">Token</label>
                        <input type="text" className="form-control" name="inputToken" value={inputToken} onChange={this.handleChange} />
                        {submitted && !inputToken &&
                            <div className="help-block">Token is required</div>
                        }
                    </div>
                    <div className="form-group">
                        <button className="btn btn-primary" disabled={loading}>Login</button>
                        {loading &&
                            <img src="data:image/gif;base64,R0lGODlhEAAQAPIAAP///wAAAMLCwkJCQgAAAGJiYoKCgpKSkiH/C05FVFNDQVBFMi4wAwEAAAAh/hpDcmVhdGVkIHdpdGggYWpheGxvYWQuaW5mbwAh+QQJCgAAACwAAAAAEAAQAAADMwi63P4wyklrE2MIOggZnAdOmGYJRbExwroUmcG2LmDEwnHQLVsYOd2mBzkYDAdKa+dIAAAh+QQJCgAAACwAAAAAEAAQAAADNAi63P5OjCEgG4QMu7DmikRxQlFUYDEZIGBMRVsaqHwctXXf7WEYB4Ag1xjihkMZsiUkKhIAIfkECQoAAAAsAAAAABAAEAAAAzYIujIjK8pByJDMlFYvBoVjHA70GU7xSUJhmKtwHPAKzLO9HMaoKwJZ7Rf8AYPDDzKpZBqfvwQAIfkECQoAAAAsAAAAABAAEAAAAzMIumIlK8oyhpHsnFZfhYumCYUhDAQxRIdhHBGqRoKw0R8DYlJd8z0fMDgsGo/IpHI5TAAAIfkECQoAAAAsAAAAABAAEAAAAzIIunInK0rnZBTwGPNMgQwmdsNgXGJUlIWEuR5oWUIpz8pAEAMe6TwfwyYsGo/IpFKSAAAh+QQJCgAAACwAAAAAEAAQAAADMwi6IMKQORfjdOe82p4wGccc4CEuQradylesojEMBgsUc2G7sDX3lQGBMLAJibufbSlKAAAh+QQJCgAAACwAAAAAEAAQAAADMgi63P7wCRHZnFVdmgHu2nFwlWCI3WGc3TSWhUFGxTAUkGCbtgENBMJAEJsxgMLWzpEAACH5BAkKAAAALAAAAAAQABAAAAMyCLrc/jDKSatlQtScKdceCAjDII7HcQ4EMTCpyrCuUBjCYRgHVtqlAiB1YhiCnlsRkAAAOwAAAAAAAAAAAA==" />
                        }
                    </div>
                    {}
                    {error &&
                        <div className={'alert alert-danger'}>{error}</div>
                    }
                </form>
                <div>
                { seconds === 0
                    ? <div>
                        <h3>Token expired! Please generate a new one:</h3>
                        <button className="btn btn-primary" disabled={loading} onClick={()=>this.regenerateToken(user.userId)}>Regenerate Token</button>
                    </div>
                    : <h3>Token Valability: {seconds < user.seconds ? `${seconds}` : seconds}</h3>
                }
                </div>
            </div>
        );
    }
}

export { TwoFactorAuthPage }; 