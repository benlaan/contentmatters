<h2>Create a New Account</h2>
<p>Use the form below to create a new account.</p>
<p>Passwords are required to be a minimum of $html.Encode(ViewData["PasswordLength"]) characters in length.</p>
<form>
    <div>
        <fieldset>
            <legend>Account Information</legend>
            <p>
                <label for="username">Username:</label>
                $html.TextBox("username") 
            </p>
            <p>
                <label for="email">Email:</label>
                $html.TextBox("email") 
            </p>
            <p>
                <label for="password">Password:</label>
                $html.Password("password") 
            </p>
            <p>
                <label for="confirmPassword">Confirm password:</label>
                $html.Password("confirmPassword") 
            </p>
            <p>
                <input type="submit" value="Register" />
            </p>
        </fieldset>
    </div>
</form>