# dsider-frontend
Repo for Front end applications for dsider

# Auth flow report

`[F]` means frontend, `[B]` means backend.

## Login

- `[F]` User sends credentials with email and password.
- `[B]` If the credentials are correct, then update `tokenValidationTime` field in `users` table and return `currentUser`. If it isn't, then return a error msg.

    ```csharp
    currentUser = collection[0];
    var filter = Builders<users>.Filter.Where(p => p.userID == currentUser.userID);
    var updatestatement = Builders<users>.Update.Set("tokenValidationTime", DateTime.Now.AddDays(5));
    var result = mongoDatabase.GetCollection<users>("users").UpdateMany(filter, updatestatement);
    ```

- `[F]` After login, Cookie is created and then redirects to the `Home` page. If login fails, then an alert shows up with error msg.

    ```JavaScript
    success: function (response) {
        if (response.userName != null) {
            createCookie('userName', response.userName, 20);
            document.location.href = '/Home';
        }
        else {
            alertify.error('Username or Password is not correct.');
        }
    }
    ```

- `[B]` Check whether if user is logged in or not:

    ```csharp
    string userName = Request.Cookies["userName"];
    users mUser = AppSetting.checkUserValidation(userName);
    if (mUser != null && mUser.userName != null)
    {
        AppSetting.saveUserLog(userName.ToLower(), "Projects", "View", "");
        return View();
    }
    else {
        return Redirect("~/Account/Login");
    }
    ```

    ```csharp
    public users checkUserValidation(string userName)
    {
        users mUser = new users();
        try
        {
            mongoDatabase = GetMongoDatabase();
            var collection = mongoDatabase.GetCollection<users>("users").Aggregate().Match(c => c.userName.ToLower() == userName.ToLower()).ToList();
            if (collection.Count > 0)
            {
                if (DateTime.Parse(collection[0].tokenValidationTime) >= DateTime.Now)
                    mUser = collection[0];
            }
        }
        catch (Exception)
        {
            return mUser;
        }
        return mUser;
    }
    ```

## Register & Logout

There are no Register and Logout features in this project.

## My opinions for auth

- Login
  - `[F]` There are no validations like valid email, password length, etc.
  - `[B]` In `users` collection, there are no required fields like `fullname`, `role`, `created_at`, `updated_at` and so on.
  - `[B]` There is no password encryption. This means you can see plain text in database. We should use `hash` to encrypt password.
  - `[B]` At the backend, you check the cookie using username. But this is not secure. We should set SessionID to cookie.

