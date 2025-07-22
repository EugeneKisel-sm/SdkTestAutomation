#!/usr/bin/env python3

import argparse
import json
import sys
import os
from typing import Dict, Any, Optional

try:
    from conductor.client.http.api_client import ApiClient
    from conductor.client.http.api.event_resource_api import EventResourceApi
    from conductor.client.http.api.workflow_resource_api import WorkflowResourceApi
    from conductor.client.http.models.event_handler import EventHandler
    from conductor.client.http.models.workflow_status import WorkflowStatus
    from conductor.client.http.models.action import Action
    from conductor.client.http.models.start_workflow import StartWorkflow
except ImportError as e:
    print(f"Error importing Conductor SDK: {e}", file=sys.stderr)
    print("Please install conductor-python: pip install conductor-python", file=sys.stderr)
    sys.exit(1)


class SdkResponse:
    def __init__(self, success: bool, content: str = "", status_code: int = 200, data: Any = None, error_message: str = ""):
        self.status_code = status_code
        self.success = success
        self.data = data
        self.content = content
        self.error_message = error_message
    
    def to_dict(self):
        return {
            "statusCode": self.status_code,
            "success": self.success,
            "data": self.data,
            "content": self.content,
            "errorMessage": self.error_message
        }


def main():
    parser = argparse.ArgumentParser(description='Python Conductor SDK Test Wrapper')
    parser.add_argument('--operation', required=True, help='SDK operation')
    parser.add_argument('--parameters', required=True, help='JSON parameters')
    parser.add_argument('--resource', required=True, help='Resource type')
    
    args = parser.parse_args()
    
    try:
        # Create ApiClient without server_url parameter
        api_client = ApiClient()
        
        # Set the host from environment variable if provided
        server_url = os.getenv('CONDUCTOR_SERVER_URL')
        if server_url:
            api_client.configuration.host = server_url
        
        event_api = EventResourceApi(api_client)
        workflow_api = WorkflowResourceApi(api_client)
        
        result = execute_sdk_operation(args.operation, args.parameters, args.resource, event_api, workflow_api)
        print(json.dumps(result.to_dict()))
    except Exception as e:
        error = SdkResponse(success=False, error_message=str(e), status_code=500)
        print(json.dumps(error.to_dict()))
        sys.exit(1)


def execute_sdk_operation(operation: str, parameters: str, resource: str, 
                         event_api: EventResourceApi, workflow_api: WorkflowResourceApi) -> SdkResponse:
    params = json.loads(parameters)
    
    if operation == "add-event":
        return add_event(params, event_api)
    elif operation == "get-event":
        return get_event(params, event_api)
    elif operation == "get-event-by-name":
        return get_event_by_name(params, event_api)
    elif operation == "update-event":
        return update_event(params, event_api)
    elif operation == "delete-event":
        return delete_event(params, event_api)
    elif operation == "get-workflow":
        return get_workflow(params, workflow_api)
    else:
        raise ValueError(f"Unknown operation: {operation}")


def create_simple_action() -> Action:
    """Create a simple action for the event handler"""
    start_workflow = StartWorkflow(
        name="test_workflow",
        version=1,
        input={}
    )
    
    return Action(
        action="start_workflow",
        start_workflow=start_workflow
    )


def add_event(parameters: Dict[str, Any], event_api: EventResourceApi) -> SdkResponse:
    try:
        event_handler = EventHandler(
            name=parameters.get("name", ""),
            event=parameters.get("event", ""),
            active=parameters.get("active", False),
            actions=[create_simple_action()]
        )
        
        event_api.add_event_handler(event_handler)
        
        return SdkResponse(success=True, status_code=200)
    except Exception as e:
        return SdkResponse(success=False, error_message=str(e), status_code=500)


def get_event(parameters: Dict[str, Any], event_api: EventResourceApi) -> SdkResponse:
    try:
        events = event_api.get_event_handlers()
        content = json.dumps([event.to_dict() for event in events])
        
        return SdkResponse(success=True, content=content, status_code=200, data=events)
    except Exception as e:
        return SdkResponse(success=False, error_message=str(e), status_code=500)


def get_event_by_name(parameters: Dict[str, Any], event_api: EventResourceApi) -> SdkResponse:
    try:
        event_name = parameters.get("event", "")
        active_only = parameters.get("activeOnly")
        
        # Fix: Pass only the required parameters
        events = event_api.get_event_handlers_for_event(event_name, active_only)
        content = json.dumps([event.to_dict() for event in events])
        
        return SdkResponse(success=True, content=content, status_code=200, data=events)
    except Exception as e:
        return SdkResponse(success=False, error_message=str(e), status_code=500)


def update_event(parameters: Dict[str, Any], event_api: EventResourceApi) -> SdkResponse:
    try:
        event_handler = EventHandler(
            name=parameters.get("name", ""),
            event=parameters.get("event", ""),
            active=parameters.get("active", False),
            actions=[create_simple_action()]
        )
        
        event_api.update_event_handler(event_handler)
        
        return SdkResponse(success=True, status_code=200)
    except Exception as e:
        return SdkResponse(success=False, error_message=str(e), status_code=500)


def delete_event(parameters: Dict[str, Any], event_api: EventResourceApi) -> SdkResponse:
    try:
        event_name = parameters.get("name", "")
        
        event_api.remove_event_handler_status(event_name)
        
        return SdkResponse(success=True, status_code=200)
    except Exception as e:
        return SdkResponse(success=False, error_message=str(e), status_code=500)


def get_workflow(parameters: Dict[str, Any], workflow_api: WorkflowResourceApi) -> SdkResponse:
    try:
        workflow_id = parameters.get("workflowId", "")
        
        workflow = workflow_api.get_execution_status(workflow_id)
        content = json.dumps(workflow.to_dict())
        
        return SdkResponse(success=True, content=content, status_code=200, data=workflow)
    except Exception as e:
        return SdkResponse(success=False, error_message=str(e), status_code=500)


if __name__ == '__main__':
    main() 