# Messages System API
Messages is a System Level API in TrakHound used to send/receive messages in a way that mimics the MQTT protocol.

## Subscribe
The Subscribe endpoint is served using a WebSockets connection. The list of Topics to subscribe are passed in the initial Request Body message. Messages are received in the format below:

### HTTP Request
```
ws://localhost:8472/_messages/subscribe?brokerId=b-1234&requestBody=true
```

<table style="width: 100%;">
    <thead>
        <tr>
            <th style="text-align: left;width: 120px;">Parameter</th>
            <th style="text-align: left;width: 170px;">DataType</th>
            <th style="text-align: left;width: 120px;">Required</th>
            <th style="text-align: left;">Description</th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td>brokerId</td>
            <td>String</td>
            <td>YES</td>
            <td>The ID of the Broker to subscribe to</td>
        </tr>  
        <tr>
            <td>clientId</td>
            <td>String</td>
            <td>NO</td>
            <td>The ID of the Client to use when establishing a connection to the broker</td>
        </tr>   
        <tr>
            <td>qos</td>
            <td>Integer (8)</td>
            <td>NO</td>
            <td>The Quality of Service to use to receive messages</td>
        </tr>   
    </tbody>
</table>

#### Request Body
The Request Body message is used to send a JSON array of the Topics to subscribe to. This is the first message that a client will send to the server and should be sent immediately upon connection.

```
["Test/#"]
```

### Response
The Message Response uses a simple Headers & Body format as shown below:
```
[TOPIC]\r
[SENDER_ID]\r
[RETAIN]\r
[QOS]\r
[TIMESTAMP]\r
\r\n
[CONTENT_BYTES]
```
<table style="width: 100%;">
    <thead>
        <tr>
            <th style="text-align: left;width: 120px;">Property</th>
            <th style="text-align: left;width: 170px;">DataType</th>
            <th style="text-align: left;width: 120px;">Required</th>
            <th style="text-align: left;">Description</th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td>TOPIC</td>
            <td>String</td>
            <td>YES</td>
            <td>The Topic for the message</td>
        </tr> 
        <tr>
            <td>SENDER_ID</td>
            <td>String</td>
            <td>NO</td>
            <td>The ID of the Sender that sent the message</td>
        </tr>  
        <tr>
            <td>RETAIN</td>
            <td>Boolean</td>
            <td>YES</td>
            <td>A Flag to determine whether the message was retained (true) or not (false)</td>
        </tr>   
        <tr>
            <td>QOS</td>
            <td>Integer (8)</td>
            <td>YES</td>
            <td>A numeric indicator to determine what Quality Of Service was used to receive the message. 0, 1, or 2</td>
        </tr>   
        <tr>
            <td>TIMESTAMP</td>
            <td>Integer (64)</td>
            <td>YES</td>
            <td>The timestamp (in UNIX nanoseconds) associated with the message</td>
        </tr>    
        <tr>
            <td>CONTENT_BYTES</td>
            <td>Byte[]</td>
            <td>YES</td>
            <td>The content of the message</td>
        </tr> 
    </tbody>
</table>

#### Example (text/plain)
```
Test/Messages
s-0025
0
0
1731499294317270500

My Name is John Doe
```

#### Example (application/json)
```
Test/Messages
s-0025
0
0
1731499294317270500

{"FirstName": "John", "LastName": "Doe"}
```

#### Example without Sender ID (text/plain)
```
Test/Messages

0
0
1731499294317270500

My Name is John Doe
```

## Publish
Messages are written to a Broker using an HTTP POST Request using the format below.

### HTTP Request
```
POST : http://localhost:8472/_messages/publish?brokerId=b-1234&topic=Test/Messages
```

<table style="width: 100%;">
    <thead>
        <tr>
            <th style="text-align: left;width: 120px;">Parameter</th>
            <th style="text-align: left;width: 170px;">DataType</th>
            <th style="text-align: left;width: 120px;">Required</th>
            <th style="text-align: left;">Description</th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td>brokerId</td>
            <td>String</td>
            <td>YES</td>
            <td>The ID of the Broker to publish the message to</td>
        </tr>  
        <tr>
            <td>topic</td>
            <td>String</td>
            <td>YES</td>
            <td>The Topic to publish the message to</td>
        </tr>
        <tr>
            <td>retain</td>
            <td>Boolean</td>
            <td>NO</td>
            <td>A flag to determine whether the Broker should retain the message (true) or not (false). Default = false</td>
        </tr>   
        <tr>
            <td>qos</td>
            <td>Integer (8)</td>
            <td>NO</td>
            <td>The Quality of Service to use when publishing the message to the broker. 0, 1, or 2</td>
        </tr>   
        <tr>
            <td>routerId</td>
            <td>String</td>
            <td>NO</td>
            <td>The Name or ID of the TrakHound Router to use to process the request</td>
        </tr>   
    </tbody>
</table>

#### Request Body
The Request Body contains the raw bytes to store as the Message Content.

## List Brokers
A list of Brokers can be returned using an HTTP GET Request using the format below.

### HTTP Request
```
GET : http://localhost:8472/_messages/brokers
```

<table style="width: 100%;">
    <thead>
        <tr>
            <th style="text-align: left;width: 120px;">Parameter</th>
            <th style="text-align: left;width: 170px;">DataType</th>
            <th style="text-align: left;width: 120px;">Required</th>
            <th style="text-align: left;">Description</th>
        </tr>
    </thead>
    <tbody>   
        <tr>
            <td>routerId</td>
            <td>String</td>
            <td>NO</td>
            <td>The Name or ID of the TrakHound Router to use to process the request</td>
        </tr>   
    </tbody>
</table>

## Query Brokers
A list of Brokers can be returned using an HTTP POST Request using the format below.

### HTTP Request
```
POST : http://localhost:8472/_messages/brokers
```

<table style="width: 100%;">
    <thead>
        <tr>
            <th style="text-align: left;width: 120px;">Parameter</th>
            <th style="text-align: left;width: 170px;">DataType</th>
            <th style="text-align: left;width: 120px;">Required</th>
            <th style="text-align: left;">Description</th>
        </tr>
    </thead>
    <tbody>   
        <tr>
            <td>routerId</td>
            <td>String</td>
            <td>NO</td>
            <td>The Name or ID of the TrakHound Router to use to process the request</td>
        </tr>   
    </tbody>
</table>

#### Request Body
The Request Body contains a JSON array of the Broker IDs to return

## List Senders
A list of Senders can be returned using an HTTP GET Request using the format below.

### HTTP Request
```
GET : http://localhost:8472/_messages/senders
```

<table style="width: 100%;">
    <thead>
        <tr>
            <th style="text-align: left;width: 120px;">Parameter</th>
            <th style="text-align: left;width: 170px;">DataType</th>
            <th style="text-align: left;width: 120px;">Required</th>
            <th style="text-align: left;">Description</th>
        </tr>
    </thead>
    <tbody>   
        <tr>
            <td>routerId</td>
            <td>String</td>
            <td>NO</td>
            <td>The Name or ID of the TrakHound Router to use to process the request</td>
        </tr>   
    </tbody>
</table>

## Query Senders
A list of Senders can be returned using an HTTP POST Request using the format below.

### HTTP Request
```
POST : http://localhost:8472/_messages/senders
```

<table style="width: 100%;">
    <thead>
        <tr>
            <th style="text-align: left;width: 120px;">Parameter</th>
            <th style="text-align: left;width: 170px;">DataType</th>
            <th style="text-align: left;width: 120px;">Required</th>
            <th style="text-align: left;">Description</th>
        </tr>
    </thead>
    <tbody>   
        <tr>
            <td>routerId</td>
            <td>String</td>
            <td>NO</td>
            <td>The Name or ID of the TrakHound Router to use to process the request</td>
        </tr>   
    </tbody>
</table>

#### Request Body
The Request Body contains a JSON array of the Sender IDs to return
