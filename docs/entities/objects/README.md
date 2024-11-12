# Objects
Object is the primary entity class and is used to represent Physical or Logical items. Each Object can be located at the root level or can be nested underneath a Parent Object. Objects are used to create an index for data. A good way to think about it is Objects are like a hierarchical tree containing primary keys for a database. An Object itself doesn't contain any data but it provides context for its Content.

## Content
Each Object has a ContentType associated with it that designates what kind of data the object is used to provide context for. A few ContentType examples are listed below:

The content for an Object is also stored as an Entity and is linked to the Object using an ObjectUuid property.

Each entity is designed to be a minimal and generic schema for the kind of data it contains. 
        For example, you would not need to have two entity classes with the same exact schema. 
        An Observation is an entity that has a scalar value and a timestamp. 
        You would not have an entity class for temperature and a separate entity class for humidity as they are both typically collected as timeseries data and would both be appropriate as an Observation. 
        The Object would determine what the data represents (this is key in understanding how TrakHound works).

<table>
    <thead>
        <tr>
            <th style="text-align: left;min-width: 100px;">ContentType</th>
            <th style="text-align: left;">Description</th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td style="vertical-align: top;"><a href="assignment.md">Assignment</a></td>
            <td>List of "member" Objects that are temporarily assigned to the "assignee" Object. This can be used for tracking time for operator logins, operations assigned to a workstation, inventory, etc.</td>
        </tr> 
        <tr>
            <td style="vertical-align: top;"><a href="blob.md">Blob</a></td>
            <td>Contains raw byte data. Used for storing images, files, etc.</td>
        </tr>
        <tr>
            <td style="vertical-align: top;"><a href="boolean.md">Boolean</a></td>
            <td>Single boolean (true/false) value</td>
        </tr> 
        <tr>
            <td style="vertical-align: top;"><a href="directory">Directory</a></td>
            <td>Contains other Objects and is used to add context</td>
        </tr>
        <tr>
            <td style="vertical-align: top;"><a href="duration.md">Duration</a></td>
            <td>Single value representing a span of time</td>
        </tr> 
        <tr>
            <td style="vertical-align: top;"><a href="event.md">Event</a></td>
            <td>Timeseries data in the form of References to an Object with a specified Timestamp. Interpreted as an Object that occurred at the specified Timestamp</td>
        </tr> 
        <tr>
            <td style="vertical-align: top;"><a href="group.md">Group</a></td>
            <td>List of other "member" Objects</td>
        </tr> 
        <tr>
            <td style="vertical-align: top;"><a href="hash.md">Hash</a></td>
            <td>List of key/value pairs</td>
        </tr> 
        <tr>
            <td style="vertical-align: top;"><a href="log.md">Log</a></td>
            <td>Log entries using a specified Log Level, Code, and Message</td>
        </tr> 
        <tr>
            <td style="vertical-align: top;"><a href="message.md">Message</a></td>
            <td>Simple messaging using an MQTT style protocol</td>
        </tr>   
        <tr>
            <td style="vertical-align: top;"><a href="message.md">MessageQueue</a></td>
            <td>Message queue using an AMQP Direct Queue style protocol</td>
        </tr>
        <tr>
            <td style="vertical-align: top;"><a href="number.md">Number</a></td>
            <td>Single numerical value with a specified Data Type</td>
        </tr> 
        <tr>
            <td style="vertical-align: top;"><a href="observation.md">Observation</a></td>
            <td>Timeseries data with a Batch ID, Sequence, and Value</td>
        </tr> 
        <tr>
            <td style="vertical-align: top;"><a href="reference.md">Reference</a></td>
            <td>Pointer to single Object</td>
        </tr>       
        <tr>
            <td style="vertical-align: top;"><a href="set.md">Set</a></td>
            <td>List of string values</td>
        </tr> 
        <tr>
            <td style="vertical-align: top;"><a href="state.md">State</a></td>
            <td>Timeseries data that records a Definition and TTL that represents a Status</td>
        </tr>
        <tr>
            <td style="vertical-align: top;"><a href="string.md">String</a></td>
            <td>Single string value</td>
        </tr>
        <tr>
            <td style="vertical-align: top;"><a href="statistic.md">Statistic</a></td>
            <td>Timeseries data with a "TimeRange" associated with it. This is used for recording metrics such as OEE that represents a value over a period of time</td>
        </tr>      
        <tr>
            <td style="vertical-align: top;"><a href="timestamp.md">Timestamp</a></td>
            <td>Single value representing a timestamp</td>
        </tr>
        <tr>
            <td style="vertical-align: top;"><a href="vocabulary.md">Vocabulary</a></td>
            <td>Single reference to a Definition</td>
        </tr> 
        <tr>
            <td style="vertical-align: top;"><a href="vocabulary-set.md">VocabularySet</a></td>
            <td>List of references to Definitions</td>
        </tr>       
    </tbody>
</table>

## Path
An Object can be queried by its "Path" which acts similar to a path in a Filesystem. An Object's "Path" is determined by the hierarchy and is constructed using the Name and ParentUuid properties.

## UUID
Each entity has a UUID property that uniquely identifies that single entity. This means that each Observation entity would have its own UUID. The UUID is deterministic based on the entity properties that would make it unique. 

An example would be an Observation where its UUID would be in the format of "ObjectUuid:Timestamp" since one of the characteristics of an Observation is that only a single value can exist per Timestamp. Other entities have different characteristics but follow a similar UUID format.

## Definition
An Object's Type is assigned using the "DefinitionUuid" property. The DefinitionUuid links to a Definition entity which contains the Type, Description, and any Metadata associated with it. Definitions also use a hierarchical model although they work slightly differently as the hierarchy is used to determine the Inheritance. Definitions can "inherit" from other definitions similar to how Object Oriented programming languages have classes that inherit from other classes. This inheritance is primarily used for associating a Definition with multiple Types but can also be used to cascade Metadata.

More information about Types and Type Inheritance will be covered in later articles but a simple overview is that Types are used to query for Objects that represent similar data but may exist at different levels in a hierarchy. Basically, you can query based on Hierarchy or by Type as both provide context to an Object.
