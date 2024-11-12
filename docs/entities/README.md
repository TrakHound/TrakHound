# Entities
Entities are the core data structures in TrakHound where multiple entities can be used to create complex models.

<table style="width: 100%;">
    <thead>
        <tr>
            <th style="text-align: left;width: 120px;">Name</th>
            <th style="text-align: left;">Link</th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td><a href="objects">Objects</a></td>
            <td>Represents Physical or Logical items.</td>
        </tr>      
        <tr>
            <td><a href="definitions">Definitions</a></td>
            <td>Type definitions that can be used to add context</td>
        </tr> 
        <tr>
            <td><a href="sources">Sources</a></td>
            <td>Record of where data was sourced from</td>
        </tr>    
    </tbody>
</table>

## Introduction

Think of TrakHound Entities as an **Abstract Database** that any App, Api, Function, or Service can interact with without needing to comply with a specific database or other infrastructure. 
This is very important in order to implement the requirement of the framework that Apps, Apis, Functions, and Services are never directly communicating with Infrastructure. 
In older architectures, a monolithic database was what all applications communicated with but in modern distributed architectures, this "Single Source" of data was separated into microservices, multiple databases, cloud services, etc. 
which required keeping up with connection strings, endpoints, etc. and converted what used to be a "Single Source" into a "Point-to-Point" architecture.

TrakHound Entities aims to "seem" like a monolithic database to Apps and Services but to support modern distributed architecture on the backend.

> [!NOTE]
> "Is TrakHound Entities a database?" Well kind of, it acts like a database to Apps, Apis, Functions, and Services but it uses Drivers to actually store/transmit data to Infrastructure. Entities is simply a logical layer in the framework meant to isolate Apps, Apis, Functions, and Services from SQL databases, MQTT brokers, etc.

Entities are to TrakHound, what DataTypes are to Databases. Meaning, you create a table in a database by combining multiple DataTypes and you create a model in TrakHound by combining multiple Entities.

Entities are divided up by Category and by Class. Categories include Objects, Definitions, and Sources. Each category has its own list of classes.

Data models are broken up into multiple entities where each entity has the following characteristics:
- A globally unique ID that points only to a single entity
- Single level (no hierarchical data within a single entity)
- Each entity must have a Source associated with it to determine where data originated from

This results in a flat & normalized schema that works well with any data storage system (Relational Database, Key/Value, etc.).

As opposed to a system where the schema of the model is restricted to a specific standard (and all input data must comply with that standard), this allows for any type of custom schema and allows for multiple schemas to exist and interact together.

> [!NOTE]
> It is important to note that there is no actual "schema" at the Entity Layer of the TrakHound Framework. This allows for true "Edge Driven" systems where the publisher determines the structure of the model (or at least the section of the model that it is publishing to). Schemas and Validation happen at a higher layer and will be discussed in a later article.

## Timestamps
Timestamps used in TrakHound Entities are represented in the form of **Unix Nanoseconds** which are 1/1,000,000,000 of a second.

<table style="width: 100%;">
    <tbody>
        <tr>
            <td style="width: 120px;"><b>ISO 8601</b></td>
            <td>2024-06-21T00:34:47.725497300-04:00</td>
        </tr>        
        <tr>
            <td style="width: 120px;"><b>TrakHound (ns)</b></td>
            <td>1718944487725497300</td>
        </tr>
    </tbody>
</table>

A Unix format was chosen as opposed to a string (ex. ISO 8601) since it is a way to standardize timestamps across multiple Infrastructure systems such as Databases. Depending on the database, it may handle DateTime types differently (or not at all) so storing the timestamp as a 64 bit integer is something any system should support.  This also takes out any confusion about Time Zones.


