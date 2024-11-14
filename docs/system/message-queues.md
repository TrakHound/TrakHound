# MessageQueues System API
MessageQueues is a System Level API in TrakHound used to send/receive messages in a way that mimics an AMQP Direct Queue.

## Subscribe
The Subscribe endpoint is served using a WebSockets connection. The Queue is passed in the URL. Messages are received in the format below:

### HTTP Request

**Subscribe to '0123456'**
```
ws://localhost:8472/_message-queues/subscribe?queue=0123456
```

**Subscribe to '0123456' and don't Auto-Acknowledge**
```
ws://localhost:8472/_message-queues/subscribe?queue=0123456&acknowledge=false
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
            <td>queue</td>
            <td>String</td>
            <td>YES</td>
            <td>The ID of the Queue to subscribe to</td>
        </tr>  
        <tr>
            <td>acknowledge</td>
            <td>Boolean</td>
            <td>NO</td>
            <td>A flag to determine whether to Auto-Acknowledge (true) or not (false). Default = true</td>
        </tr>    
    </tbody>
</table>

### Response
The Message Response uses a simple Headers & Body format as shown below:
```
[DELIVERY_ID]\r
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
            <td>DELIVERY_ID</td>
            <td>String</td>
            <td>YES</td>
            <td>The ID to use when Acknowledging or Rejecting the message manually</td>
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
0123456:1
1731499294317270500

My Name is John Doe
```

#### Example (application/json)
```
0123456:1
1731499294317270500

{"FirstName": "John", "LastName": "Doe"}
```

## Publish
Messages are written to a Queue using an HTTP POST Request using the format below.

### HTTP Request
```
POST : http://localhost:8472/_message-queues/publish?queue=0123456
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
            <td>queue</td>
            <td>String</td>
            <td>YES</td>
            <td>The ID of the Queue to publish the message to</td>
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

## Pull
A single message can be pulled from a Queue using an HTTP GET Request using the format below.

### HTTP Request
```
GET : http://localhost:8472/_message-queues?queue=0123456
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
            <td>queue</td>
            <td>String</td>
            <td>YES</td>
            <td>The ID of the Queue to pull the message from</td>
        </tr>   
        <tr>
            <td>acknowledge</td>
            <td>Boolean</td>
            <td>NO</td>
            <td>A flag to determine whether to Auto-Acknowledge (true) or not (false). Default = true</td>
        </tr>    
    </tbody>
</table>

## Acknowledge
A message that has been pulled from a Queue can be manually Acknowledged using an HTTP PUT Request using the format below.

### HTTP Request
```
PUT : http://localhost:8472/_message-queues/acknowledge?queue=0123456&deliveryId=0123456:1
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
            <td>queue</td>
            <td>String</td>
            <td>YES</td>
            <td>The ID of the message was pulled from from</td>
        </tr>   
        <tr>
            <td>deliveryId</td>
            <td>Boolean</td>
            <td>NO</td>
            <td>The ID that was received when the message was pulled</td>
        </tr>    
    </tbody>
</table>

## Reject
A message that has been pulled from a Queue can be manually Rejected and returned to the Queue using an HTTP PUT Request using the format below.

### HTTP Request
```
PUT : http://localhost:8472/_message-queues/reject?queue=0123456&deliveryId=0123456:1
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
            <td>queue</td>
            <td>String</td>
            <td>YES</td>
            <td>The ID of the message was pulled from from</td>
        </tr>   
        <tr>
            <td>deliveryId</td>
            <td>Boolean</td>
            <td>NO</td>
            <td>The ID that was received when the message was pulled</td>
        </tr>    
    </tbody>
</table>
