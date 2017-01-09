### CRM ++ ###

* CRM++ provides an easier way to integrate with Dynamics CRM. 

### Overview ###
Those who are familiar to using the CRM IOrganizationService interface will know that using this service with late-bound entities can be painful and difficult to test. Even using the XRM code-generation tool to generate early bound entity classes can be ugly when it comes to the naming conventions, querying and customising. CRM++ aims to solve these difficulties by allowing entities to be designed in code and providing strongly-typed methods for querying and customising.

### Quick start ###

* Reference CRM++ and use it's provided clients

```
#!c#
string myConnectionString = "Url=;Username=;Password=;authtype=;" // Populate this for your environment
ICrmPlusPlus crmPlusPlus = CrmPlusPlus.ForTenant(myConnectionString);

ICrmPlusPlusCustomizationClient customizationClient = crmPlusPlus.GetCustomizationClientForSolution(PublisherSettings.Default, SolutionSettings.Default);
ICrmPlusPlusEntityClient entityClient = crmPlusPlus.EntityClient;
```

* Design your entities in code:  

```
#!c#

using Civica.CrmPlusPlus.Sdk.EntityAttributes;
using Civica.CrmPlusPlus.Sdk.EntityAttributes.PropertyTypes;

[EntityName("new_myentity")]
[EntityInfo("My Entity", EntityAttributes.Metadata.OwnershipType.OrganizationOwned)]
public class MyEntity : CrmPlusPlusEntity
{
    [PropertyName("new_mystring")]
    [PropertyInfo("My String", EntityAttributes.Metadata.AttributeRequiredLevel.ApplicationRequired)]
    [String(255, EntityAttributes.Metadata.StringFormatName.Text)]
    public string MyString { get; set; }

    // Add more properties...
}
```

* Create it in CRM if it's not there already
```
#!c#

customizationClient.CreateEntity<MyEntity>();

// Create a specific property
customizationClient.CreateProperty<MyEntity>(e => e.MyString);

// ... Or just create all of them
customizationClient.CreateAllProperties<MyEntity>();
```

* Manipulate entity data: 

```
#!c#

// Create a record
var data = new MyEntity();
entityClient.Create(data);

// Update it
data.MyString = "Updating it now...";
entityClient.Update(data);

// Delete it 
entityClient.Delete(data);
```

* Query for records:

```
#!c#

// Get an individual record
var retrieval = Retrieval
    .ForEntity<MyEntity>(data.Id)
    .Include(e => e.MyString);

data = entityClient.Retrieve(retrieval);

// Or query for multiple
var query = Query.ForEntity<MyEntity>()
    .Include(e => e.MyString)
    .Filter(FilterType.And, filter => 
    {
        filter.Condition(e => e.MyString, ConditionOperator.Equal, "Updating it now..."); 
    });

IEnumerable<MyEntity> queryResults = entityClient.RetrieveMultiple(query);
```

### How do I get set up? ###

Download the source and build in visual studio. The CRM SDK is included in the solution nuget packages, so ensure this and other nuget packages are restored before or as part of the build.

* How to run tests
Run tests using the Visual Studio Test runner - tests are in XUnit.NET and will be discoverable once XUnit nuget packages are restored.

For the integration test project, use an instance of CRM to run the tests against. The connection string is configurable in the app.config for this project