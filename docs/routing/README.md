# Routing
Routing in the TrakHound Framework is used to route requests to the configured Drivers and is the basis for creating a layer of abstraction between applications and infrastructure.

## Drivers
Drivers are used to access data that is stored in an external database or system. Drivers are the primary layer of abstraction for TrakHound Entities. Using a driver separates applications from the external database and allows applications to be portable and databases to be swapped as needed.

Drivers are organized by their functionality and what type of data they are accessing. Drivers are designed to act similar to microservices as they allow the underlying data to be isolated from other types of data. This allows a user to configure their system to use the database or system that is the most appropriate for the specific driver as opposed to choosing a system that attempts to satify **every** function.

Each Driver satisfies a single **Route** and has at least one **Endpoint** which implements each individual function of the driver. An example of a Driver that has multiple Endpoints would be a Query driver that can query based on Object UUID or by time range. If a Driver can not satify an Endpoint, a RouteNotConfigured response can be returned.

Drivers are typically organized by the external system (ex. SqlServer, Redis, etc.), are stored as a Package, and loaded as a Module in a TrakHound Instance.

An example of a driver would be to Publish String Entities. The only functionality that the driver must have is to write the entity, not being able to read, delete, query, etc. This allows a driver to publish to a system that may not have the abiltiy to directly read the data such as a pipeline or log (ex. Kafka). This can also be used within a digital supply chain to publish data to a customer's system without needing to worry about having access/permission to read from their system.

### Example Drivers
<table style="width: 100%;">
    <thead>
        <tr>
            <th>Route</th>
            <th>Description</th>
            <th>System Examples</th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td>Objects.String.Read</td>
            <td>Read String entities with the specified UUID(s)</td>
            <td>SQL, NoSQL, Redis, etc.</td>
        </tr>
        <tr>
            <td>Objects.Boolean.Publish</td>
            <td>Write Boolean entities</td>
            <td>SQL, NoSQL, Redis, MQTT, Kafka, etc.</td>
        </tr>
        <tr>
            <td>Objects.Observation.Query</td>
            <td>Query Observation entities by ObjectUuid along with a range of time</td>
            <td>SQL, InfluxDB, TimescaleDB, etc.</td>
        </tr>
        <tr>
            <td>Blobs.Publish</td>
            <td>Write raw Blob bytes</td>
            <td>FileSystem, AWS S3, Azure Storage, etc.</td>
        </tr>
    </tbody>
</table>

## Routers
Routers are used to configure what Drivers are used to satisfy a request. A router can be configured to satisfy all routes in a Target or each individual route based on a specified pattern. Routes can also be configured to Redirect to another Target based on a specified response (ex. NotFound, NotAvailable, etc.).

### Route
A Route defines what routes a Target should satisfy based on specified pattern.

### Target
A Target defines what is used to satisfy the route. Targets can be either a Driver or another Router.