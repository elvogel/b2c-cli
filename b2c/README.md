# B2C - Command-Line Graph API Tool for Azure AD B2C

The purpose of this command-line tool is to provide quick and easy access to the creation of users and groups within an Azure AD B2C instance. This CLI utility is specifically designed to replace the [B2C GraphAPI Dotnet CLI](https://github.com/AzureADQuickStarts/B2C-GraphAPI-DotNet.git) project Microsoft gives us all on GitHub, which is helpful for sample code but unfortunately not much else.

Think of this utility as a quick and easy way to get impotant information out of Azure AD B2C without having to use the UI. 

The current scope of this project is almost exclusively limited to:

- setting up the `b2c.json` configuration file consisting of `AppId`, `Tenant` and `Secret` properties
- adding, deleting and listing users 
- adding users to groups and listing members

Adding users is really important when you're trying to test things. Adding groups, and adding users to groups, is pretty trivial from the B2C portal UI, so for sake of brevity and scope we limit our activity to adding test users to the Graph API. Adding users is most definitely the most difficult of tasks, so that's what we focus on with this utility. 

The following is a demonstration of some of the available commands.
  
```bash
Azure AD Graph Commands for B2C directories.

Usage: b2c [options] [command]

Options:
  -?|-h|--help  Show help information

Commands:
  configure     configure the graph settings
  groups        operations on groups
  users         user commands

Run 'b2c [command] --help' for more information about a command.
```

### User Commands
```bash

user commands

Usage: b2c users [options] [command]

Options:
  -?|-h|--help  Show help information

Commands:
  create        create the user
  delete        delete the user

Run 'users [command] --help' for more information about a command.

```
### Group Commands
```bash
./b2c groups -?

---
operations on groups

Usage: b2c groups [options] [command]

Options:
  -?|-h|--help  Show help information

Commands:
  list          list all available groups
  user          Add user to a group

Run 'groups [command] --help' for more information about a command.
---

./b2c groups list -?

---
list all available groups

Usage: b2c groups list [options]

Options:
  -vf|--verboseFormat  Display full, formatted JSON object
  -v|--verbose         display full JSON object properties
  -t|--time            time the operation
  -?|-h|--help         Show help information
---

./b2c groups user -?
Add user to a group

Usage: b2c groups user [options]

Options:
  -g|--group <GROUP_ID>  the group's object ID
  -u|--user <USER_ID>    the user's object ID
  -?|-h|--help           Show help information
---
```
