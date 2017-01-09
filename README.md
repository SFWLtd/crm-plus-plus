# README #

This README would normally document whatever steps are necessary to get your application up and running.

### What is this repository for? ###

* CRM++ provides easier, code first integration with Dynamics CRM.

* Design entities and define their associated metadata in c# attributes:  

```
#!c#

[EntityName("new_myentity")]
[EntityInfo("My Entity", EntityAttributes.Metadata.OwnershipType.OrganizationOwned)]
public class MyEntity : CrmPlusPlusEntity
{
    [PropertyName("new_mystring")]
    [PropertyInfo("My String", EntityAttributes.Metadata.AttributeRequiredLevel.ApplicationRequired)]
    [String(255, EntityAttributes.Metadata.StringFormatName.Text)]
    public string DisplayName { get; set; }

    // Add more properties...
}
```


### How do I get set up? ###

* Summary of set up
* Configuration
* Dependencies
* Database configuration
* How to run tests
* Deployment instructions

### Contribution guidelines ###

* Writing tests
* Code review
* Other guidelines

### Who do I talk to? ###

* Repo owner or admin
* Other community or team contact