#!/usr/bin/env python3

from typing import Dict, Any
from conductor.client.http.api.event_resource_api import EventResourceApi
from conductor.client.http.models.event_handler import EventHandler
from conductor.client.http.models.action import Action
from conductor.client.http.models.start_workflow import StartWorkflow
from ..operation_utils import execute_with_error_handling, create_sdk_configuration
from ..sdk_response import SdkResponse


def execute(operation: str, parameters: Dict[str, Any]) -> SdkResponse:
    """Execute event operations with centralized error handling"""
    return execute_with_error_handling(lambda: _execute_operation(operation, parameters))


def _execute_operation(operation: str, parameters: Dict[str, Any]) -> SdkResponse:
    """Execute the specific event operation"""
    api_client = create_sdk_configuration()
    event_api = EventResourceApi(api_client)
    
    if operation == "add-event":
        return _add_event(parameters, event_api)
    elif operation == "get-event":
        return _get_event(event_api)
    elif operation == "get-event-by-name":
        return _get_event_by_name(parameters, event_api)
    elif operation == "update-event":
        return _update_event(parameters, event_api)
    elif operation == "delete-event":
        return _delete_event(parameters, event_api)
    else:
        raise ValueError(f"Unknown event operation: {operation}")


def _create_simple_action() -> Action:
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


def _add_event(parameters: Dict[str, Any], event_api: EventResourceApi) -> SdkResponse:
    """Add a new event handler"""
    event_handler = EventHandler(
        name=parameters.get("name", ""),
        event=parameters.get("event", ""),
        active=parameters.get("active", False),
        actions=[_create_simple_action()]
    )
    
    event_api.add_event_handler(event_handler)
    return SdkResponse.create_success()


def _get_event(event_api: EventResourceApi) -> SdkResponse:
    """Get all event handlers"""
    events = event_api.get_event_handlers()
    return SdkResponse.create_success(events)


def _get_event_by_name(parameters: Dict[str, Any], event_api: EventResourceApi) -> SdkResponse:
    """Get event handlers by name"""
    event_name = parameters.get("event", "")
    active_only = parameters.get("activeOnly")
    
    events = event_api.get_event_handlers_for_event(event_name, active_only)
    return SdkResponse.create_success(events)


def _update_event(parameters: Dict[str, Any], event_api: EventResourceApi) -> SdkResponse:
    """Update an existing event handler"""
    event_handler = EventHandler(
        name=parameters.get("name", ""),
        event=parameters.get("event", ""),
        active=parameters.get("active", False),
        actions=[_create_simple_action()]
    )
    
    event_api.update_event_handler(event_handler)
    return SdkResponse.create_success()


def _delete_event(parameters: Dict[str, Any], event_api: EventResourceApi) -> SdkResponse:
    """Delete an event handler"""
    event_name = parameters.get("name", "")
    event_api.remove_event_handler_status(event_name)
    return SdkResponse.create_success() 