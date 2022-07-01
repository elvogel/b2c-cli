# B2C - Command-Line Graph API Tool for Azure AD B2C

The purpose of this command-line tool is to provide quick and easy operational ability to an Azure AD B2C instance. This CLI utility is specifically designed to replace the [B2C GraphAPI Dotnet CLI](https://github.com/AzureADQuickStarts/B2C-GraphAPI-DotNet.git) project Microsoft gives us all on GitHub, which is helpful for sample code but unfortunately not much else. 

As of April 2022, we're now also replacing IEF functionality to the buggy [Azure AD B2C Extension for Visual Studio Code](https://marketplace.visualstudio.com/items?itemName=AzureADB2CTools.aadb2c) after constantly running into issues trying to make it work. 


The current scope of this project is almost exclusively limited to:

- schema validation, compiling, and publishing IEF policies for different environments
- adding, deleting and listing users 
- adding users to groups and listing members

## Helpful Links
Outside of the [Azure AD B2C documentation](https://docs.microsoft.com/en-us/azure/active-directory-b2c/), these links should be in your arsenal of information for understanding AADB2C better.

- [Active Directory B2C Custom Policy Starter Pack](https://github.com/Azure-Samples/active-directory-b2c-custom-policy-starterpack)
- [Azure AD B2C Community](https://azure-ad-b2c.github.io/azureadb2ccommunity.io/)
- [Azure AD B2C GitHub](https://github.com/azure-ad-b2c)
- [Azure AD B2C Custom Policies with the IEF](https://github.com/Azure-Samples/active-directory-b2c-advanced-policies)
- [Azure AD B2C Custom Policies - Deep Dive on Custom Policy Schema (PDF)](https://download.microsoft.com/download/3/6/1/36187D50-A693-4547-848A-176F17AE1213/Deep%20Dive%20on%20Azure%20AD%20B2C%20Custom%20Policies/Azure%20AD%20B2C%20Custom%20Policies%20-%20Deep%20Dive%20on%20Custom%20Policy%20Schema.pdf)
- [Rory Braybrook's Blog](https://rbrayb.medium.com/) 

## Getting Started

### App Registration
You need to register this command-line tool as its own application within Azure AD B2C. This tool needs the following API permissions in your AD B2C directory:
- `AuditLog.Read.All`
- `Group.ReadWrite.All`
- `Policy.ReadWrite.TrustFramework`
- `User.ReadWrite.All`

### Configuration

The application looks for a `b2c.json` file in its directory. You can also override this by passing in the `-c` or `-cfg` parameter and specify a config file. 

This command-line tool assumes that you have several AD B2C directories for your solution, similar to how you would for any application resource, such as `Development`, `Testing`, `Production` and so forth. This is especially crucial if you are developing and testing custom IEF policies. 

The application requires a `b2c.json` configuration file to operate. It should look similar to the following:

```json
{
  "Environments": [
    {
      "Name": "Development",
      "Production": false,
      "AppId": "11111111-2222-3333-4444-660ac747fbf1",
      "Tenant": "yourtenantname.onmicrosoft.com",
      "TenantId": "22222222-3333-4444-5555-465f71ae2d0f",
      "Secret": "randomsecretfromAppregistration",
      "Settings": {
        "InstrumentationKey": "",
        "IdentityExperienceFrameworkAppId": "another-guid-from-the-directory",
        "ProxyIdentityExperienceFrameworkAppId": "Your dev environment AD Proxy app Id",
        "FacebookAppId": "0",
        "SomeCustomSettingUsedInThePolicyXmlFiles": "abcdefg",
        "IdToken": "id_token"
      }
    },
    {
      "Name": "Prod",
      "Production": true,
      "AppId": "11111111-2222-3333-4444-660ac747fbf1",
      "Tenant": "yourtenantname.onmicrosoft.com",
      "TenantId": "22222222-3333-4444-5555-465f71ae2d0f",
      "Secret": "randomsecretfromAppregistration",
      "Settings": {
        "IdentityExperienceFrameworkAppId": "Your prod environment AD app Id",
        "ProxyIdentityExperienceFrameworkAppId": "Your AD prod environment Proxy app Id",
        "FacebookAppId": "0"
      }
    }
  ]
}
```
All settings outside the `Settings` section are required values and used for administrative tasks against the directory itself. Everything inside of the `Settings` is optional and can be whatever basic value you'd like it to be. Whatever you put in here like so:


```
<TechnicalProfile Id="login-NonInteractive">
  <DisplayName>Local Account SignIn</DisplayName>
  <Protocol Name="OpenIdConnect" />
  <Metadata>
    <Item Key="UserMessageIfClaimsPrincipalDoesNotExist">We can't seem to find your account</Item>
    <Item Key="UserMessageIfInvalidPassword">Your password is incorrect</Item>
    <Item Key="UserMessageIfOldPasswordUsed">Looks like you used an old password</Item>

    <Item Key="ProviderName">https://sts.windows.net/</Item>
    <Item Key="METADATA">https://login.microsoftonline.com/{Settings:Tenant}/.well-known/openid-configuration</Item>
    <Item Key="authorization_endpoint">https://login.microsoftonline.com/{Settings:TenantId}/oauth2/token</Item>
    <Item Key="response_types">{Settings:IdToken}</Item>
    <Item Key="response_mode">query</Item>
    <Item Key="scope">email openid</Item>
```

In this example:
- we replaced `{Settings:Tenant}` and `{Settings:TenantId}` with the `Tenant` and `TenantId` settings in the `Environment` object root. This design inconsistency is for convenience - those values are required for directory operations, but it's redundant to put them in `Settings` again, so we just 'pretend' like they're there :)
- `{Settings:IdToken}` is replaced in the policy file(s) for `Developer` environment output. This setting would be need to be added to the `Prod` environment object in order to be effectively replaced there. 

If you're an advanced policy developer, here's how the values in the [Active Directory B2C Custom Policy Starter Pack](https://github.com/Azure-Samples/active-directory-b2c-custom-policy-starterpack) repository map in the code:




## Commands
The following is a demonstration of some of the available commands.
  
```bash
Usage: b2c [command] [options]

Options:
  -?|-h|--help  Show help information.

Commands:
  groups        operations on groups
  ief           Identity Experience Framework (IEF) commands
  users         user commands

Run 'b2c [command] -?|-h|--help' for more information about a command.
```

### User Commands
```bash

user commands

Usage: b2c users [command] [options]

Options:
  -?|-h|--help  Show help information.

Commands:
  create        create the user
  delete        delete the user
  list          list all users

Run 'users [command] -?|-h|--help' for more information about a command.

```
### Group Commands

```
./b2c groups -?

---
operations on groups

Usage: b2c groups [command] [options]

Options:
  -?|-h|--help  Show help information.

Commands:
  add           Add user to a group
  list          list all available groups
  listUsers     List users in a group

Run 'groups [command] -?|-h|--help' for more information about a command.
---
```
### IEF Commands

```
Identity Experience Framework (IEF) commands

Usage: b2c ief [command] [options]

Options:
  -?|-h|--help    Show help information.

Commands:
  compile         compile IEF files for all the different Environments
  publish         publish policy files (in the sequence given) the specified environment
  validateSchema  validate a file against the TrustPolicy schema

Run 'ief [command] -?|-h|--help' for more information about a command.

```
