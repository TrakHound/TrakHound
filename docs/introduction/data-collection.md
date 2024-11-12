# Data Collection & Storage
Collecting Data and Storing Data are common functionality for IIoT applications. The TrakHound Framework provides functionality to manage collection through Services and storage through Routing and Drivers.

External data can be collected by Funcions and Services that can be ran inside of a TrakHound Instance. Functions can be used to import data once or at a scheduled interval. Services can be used to establish persistent connections to a PLC, ERP, MQTT broker, or other external system. That data is then passed through the Router to then publish the data to an external Database (or other system).

> [!NOTE]
> Learn more by reading about <a href="/docs/trakhound-framework/how-it-works/routing">Routing</a> and how it works in the TrakHound Framework.

## Caching
Caching data is a crucial functionality for reponsive User Interfaces but can be difficult in IIoT applications due to the volume of data and the frequency that data changes. In the example of a typical "E-Commerce" web site, caching can be fairly simple by implementing a Database such as <string>Redis</string> and by utilizing the browser cache. 

For IIoT applications, this can get much more difficult as industrial data many times comes from multiple producers of data. An ERP, MES, or quality system might all be updating data in the same model along with high frequency device data. Using a typical caching approach, this might result in the cache either being constantly updated or could cause issues with consistentcy as multiple producers might be trying to update the same model.

TrakHound Routing solves these issues by the structure of Entity data and by only caching data that is requested. This allows for rarely accessed data to not need to be cached but frequently accessed data to remain cached but updated as new data is published.

## Cold Storage
Data in IIoT applications is often recorded for tracebility and to comply with regulations. This means that much of the data that is collected will only ever be accessed in rare conditions. These conditions could be that data is accessed only while a part is being manufactured, only if there is an issue found, or years later in case of an incident.

TrakHound Routing can be used to implement <strong>Cold Storage</strong> by publishing data directly to a long term storage while bypassing any caches or temporary databases. This data can then be retrieved using the same request as if it was cached by the Router checking each source to see if the data exists until it is found.