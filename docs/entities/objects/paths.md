# Paths
Paths are the primary mechanism to navigate Object Entities. They can be used alone or within a Query.

```
[NAMESPACE]:/[PARTIAL_PATH]
```

<table>
    <tbody>
        <tr>
            <td style="vertical-align: top;">Namespace</td>
            <td>Namespaces are used to organize Objects at the root level and can be used in Drivers to filter or partition by</td>
        </tr>    
        <tr>
            <td style="vertical-align: top;">Partial Path</td>
            <td>The Partial Path is the <b>Rooted</b> path without the Namespace. This is similar to a Filesystem path without the Drive</td>
        </tr>    
    </tbody>
</table>

## Absolute
An **Absolute Path** contains both the Namespace and Partial Path. The Partial Path must begin with a forward slash ('/') and cannot contain any Expression characters. An absolute path can be converted to an Object UUID by creating a SHA-256 hash of the lowercase absolute path. This means that using an absolute path is just as efficient as using an Object UUID directly. Absolute paths can also use the "uuid=" prefix as part of the path.

### Notes
- Must contain Namespace
- Not case sensitive
- Can be converted directly to an Object UUID

### Examples
```
Main:/Enterprise/Site/Area/Line

Debug:/Testing/StringExample

uuid=363d72f932529d42f7baa0ee0fe96dc36776aea397f4800d4419c2eeb2b59493
```

## Expression
A path can contain **Expressions** that can be used to query for Objects. These expressions can be used in any part of the path.

A path is interpreted as an Expression whenever it doesn't contain a Namespace, doesn't start with a forward slash ('/'), or contains an Expression Character listed bloew. Each part of the path is used to query for Objects based on the Object **Name** unless using the Expressions listed below:

<table>
    <thead>
        <tr>
            <th style="text-align: left;min-width: 200px;">Expression</th>
            <th style="text-align: left;">Description</th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td style="vertical-align: top;">*</td>
            <td>Wildcard that matches any Object at one level deep. Note: When used alone, this will match ANY Object at any level</td>
        </tr>    
        <tr>
            <td style="vertical-align: top;">**</td>
            <td>Wildcard that matches any Object at any level</td>
        </tr>    
        <tr>
            <td style="vertical-align: top;">..</td>
            <td>Navigates one level above</td>
        </tr>    
        <tr>
            <td style="vertical-align: top;">type={TYPE}</td>
            <td>Queries Objects that match the specified TYPE. This also matches any types that inherit from the specified TYPE</td>
        </tr>    
        <tr>
            <td style="vertical-align: top;">meta@name={METADATA_NAME}</td>
            <td>Queries Objects that contain metadata with the matching Name as the specified METADATA_NAME</td>
        </tr>    
        <tr>
            <td style="vertical-align: top;">~{NAME}*</td>
            <td>Queries Objects that are **LIKE** the specified NAME. This operates similar to a SQL LIKE query and uses the '*' character as a wildcard</td>
        </tr>       
    </tbody>
</table>

### Notes
- Used to query multiple Objects
- Is converted to individual client requests
- Not case sensitive

### Example 1
Match any Object that has a Name = 'MTConnect'
```
mtconnect
```

### Example 2
Match any Object that has a Name = 'MTConnect' under the base path of /ShopFloor
```
/shopfloor/**/mtconnect
```

### Example 3
Match any Object that has a Parent UUID equal to 'e79463b44d9d9751a8c38d31f6ad07dd644ceca4c4cd57f78ca23059a0e3eb0c'
```
uuid=e79463b44d9d9751a8c38d31f6ad07dd644ceca4c4cd57f78ca23059a0e3eb0c/*
```

### Example 4
Match any Object that has a Definition Type equal to 'Execution'
```
type=Execution
```


