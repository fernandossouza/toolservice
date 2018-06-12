# toolservice

management of tools

## ToolType

this used for CRUD of tool type

* id: id tool type.
  * Integer
  * Ignored on Create, mandatory on the other methods
* name: name of tool type
  * String (50 characters)
  * Required
* description: description of tool type
  * String (100 characters)
* thingGroupIds: array with id thing group
  * int
* status: status of tool type
* lifeCycle: life time total
  * Double
  * Required
* unitOfMeasurement: unit of measurement of life
  * String
  * Required

### JSON Example

```json
{
  "id": 1,
  "name": "Tipo",
  "description": "Criação tipo para as ferramentas update",
  "thingGroupIds": [2],
  "status": "available",
  "lifeCycle": 0,
  "unitOfMeasurement": "minute"
    
}
```

## Url ToolType

* api/tooltype/{optional=startat}{optional=quantity}
  * GET: Return list tooltype
  * POST: Create tooltype
* api/tooltype/{id}
  * GET: Return tooltype
  * PUT: Update tooltype
    * Body: json tooltype
  * DELETE: inactive tooltype

## Tool

this used for CRUD of tool type

* id: id tool type.
  * Integer
  * Ignored on Create, mandatory on the other methods
* name: name of tool type
  * String (50 characters)
  * Required
* description: description of tool type
  * String (100 characters)
* serialNumber: serial number of tool
  * String (100 characters)
* code: code of tool
  * String (100 characters)
* lifeCycle: life time total
  * Cannot be changed
* currentLife: life consumed of tool
  * Double
* unitOfMeasurement: unit of measurement of life
  * Cannot be changed
* position: position of the tool
  * Nullable Integer
* typeId: id of tool type
  * Integer
  * Required
* typeName: name of tool type
  * String
  * used only get (informative)
* status: status of tool
* currentThingId: Id of the current Thing that is using the tool
  * Ignored on Create and Update
  * Can only be changed when associating tools with things

### JSON Example

```json
{
  "id": 1,
  "name": "Ferramenta",
  "description": "TESTE ferramenta",
  "serialNumber": null,
  "code": null,
  "lifeCycle": 100,
  "currentLife": 0,
  "position": null,
  "unitOfMeasurement": "minute",
  "typeId": 1,
  "typeName": "Tipo",
  "status": "available",
  "currentThingId": 1
}
```

## Url Tool

* api/tool/{optional=startat}{optional=quantity}{optional=orderField}{optional=order}{optional=fieldFilter}{optional=fieldValue}
  * GET: Return list tool
    * startat: represent where the list starts t the database (Default=0)
    * quantity: number of resuls in the query (Default=50)
    * orderField: Field in which the list will be order by (Possible Values:
      Name,Description,SerialNumber,Code,UnitOfMeasurement,TypeName,typeId
      Status)(Default=id)
    * order: Represent the order of the listing (Possible Values: ascending,
      descending)(Default=Ascending)
    * fieldFilter: represents the field that will be seached (Possible Values:
      Name,Description,SerialNumber,Code,UnitOfMeasurement,TypeName
      Status) (Default=null)
    * fieldValue: represents de valued searched on the field (Default=null)
  * POST: Create tool
* api/tool/{id}
  * GET: Return tool
  * PUT: Update tool
    * Body: json tool
  * DELETE: inactive tool
* api/tool/inuse
  * GET: Return tools that are associtated with a thing.
* api/tool/thing/{thingid}
  * GET: Return tools that are associtated with the thing where thingId = thingId.

# GatewayAPI

API Responsible to provide access to information nedeed to compose the recipe
from other APIs

## URLs

* gateway/thinggroups/{optional=startat}{optional=quantity}

  * Get: Return List of Groups of Things
    * startat: represent where the list starts at the database (Default=0)
    * quantity: number of resuls in the query (Default=50)

* gateway/thinggroups/{id}

  * Get: Return Group of Things with thingGroupId = ID

* gateway/thinggroups/attachedthings/{groupid}

  * Get: List of Thing inside the group where thingGroupId = ID

* gateway/things/{id}

  * Get: Thing where thingId = ID

# StateConfigurationAPI

API to Manage The possible states for tools on Lorien. Used to read the state configuration.

## StateConfiguration Data Format

These are the fields of the StateConfiguration and it's constrains:

* states: possible states of this production order type.
  * Array State objects
* state: Value of the current state
  * Enum
  * Possible Values: available, in_use, in_maintenance, \* GET: Return list tool
    * startat: represent where the list starts t the database (Default=0)not_available, inactive
* possibleNextStates: possible values after the current one
  * List Enum
  * Possible Values: available, in_use, in_maintenance, not_available, inactive

### JSON Example:

```json
{
  "states": [
    {
      "state": "available",
      "possibleNextStates": [
        "in_use",
        "in_maintenance",
        "not_available",
        "inactive"
      ],
      "needsJustification": false
    },
    {
      "state": "in_use",
      "possibleNextStates": ["available", "in_maintenance", "not_available"],
      "needsJustification": false
    },
    {
      "state": "in_maintenance",
      "possibleNextStates": ["available", "not_available", "inactive"],
      "needsJustification": true
    },
    {
      "state": "not_available",
      "possibleNextStates": ["available"],
      "needsJustification": true
    },
    {
      "state": "inactive",
      "possibleNextStates": [],
      "needsJustification": true
    }
  ]
}
```

## URLs

* api/stateconfiguration/

  * Get: Return state configuration

#StateManagementAPI

API to Manage the current state of a tool on Lorien. Used to update the status of the tool.

## URLs

* /api/tool/statemanagement/number{serial}{state}

  * Put: Update the status of the tool where serial = serial to the Status = State (Valid States: available, in_use, in_maintenance, not_available, inactive)
    * IMPORTANT: If the status require its needed to send a justification JSON on the body:

```json
{
  "text": "teste"
}
```

* /api/tool/statemanagement/id{toolid}{state}{username}

  * Put: Update the status of the tool where toolid = toolid to the Status = State (Valid States: available, in_use, in_maintenance, not_available, inactive), username = user loged into the system
    * IMPORTANT: If the status require its needed to send a justification JSON on the body:

```json
{
  "text": "teste"
}
```

## After Status Change Post

This API can send the Data to a Endpoint if the configuration in present. The API will send a tool JSON to the configured endpoint.

# StateTransitionHistoryAPI

API to read all the transition history of a tool.

## StateTransitionHistory Data Format

These are the fields of the StateTransitionHistory and it's constrains:

* toolId: Id of the tool of this history
  * Integer
* justificationNeeded: Indicate if the Justification was needed to this transition
  * Boolean
* justification: justification object if the justification was needed
  * Justification Json
* previousState: Previous State of the tool
  * String
* nextState: New State of the tool
  * String
* timeStampTicks: Date of the transition in Ticks
  * Long
* previoustLife: Life span of the tool at the time of transition
  * Double
* tool: Tool of the history
  * Tool Json
* username: User logged into the system
  * String

```json
{
  "tool": {
    "id": 1,
    "name": "Ferramenta",
    "description": "TESTE ferramenta",
    "serialNumber": "23123",
    "code": "2323",
    "lifeCycle": 100,
    "currentLife": 0,
    "unitOfMeasurement": "minute",
    "typeId": 1,
    "typeName": "Tipo",
    "status": "available",
    "username": "fernando"
  },
  "toolId": 1,
  "justificationNeeded": true,
  "justification": null,
  "previousState": "available",
  "nextState": "in_maintenance",
  "previoustLife": 50005,
  "timeStampTicks": 636506700576991715
}
```

## URLs

* /api/tool/StateTransitionHistory/{toolid}{from}{to}
  * GET: Return list tool history
    * toolid: Mandatory Id of the searched tool
    * from: Initial date of the search (Default: 00:00 of the current Day)
    * to: End date of the search (Default: 23:59 of the current Day)

# AssociateToolAPI

API used to associate a tool with a thing in a certain position. When the operation is done the API set the status of the Tool and save the history.

## URLs

* api/tool/AssociateTool/associate/{thingId}{toolid}{position}
  * Put: Associate the Tool where toolid=toolId to the thing where thingid=thingid in the position where position=position IF THIS IS PERMITED.
* api/tool/AssociateTool/disassociate/
  * Put: Associate the Tool where toolid=toolId to the thing where thingid=thingid IF THIS IS PERMITED.
    * Body: Tool JSON (Current Life MUST BE LESSER OR EQUAL THAN CURRENT LIFE)

## After Association Post

This API can send the Data to a Endpoint if the configuration in present. The API will send a tool JSON to the configured endpoint.
