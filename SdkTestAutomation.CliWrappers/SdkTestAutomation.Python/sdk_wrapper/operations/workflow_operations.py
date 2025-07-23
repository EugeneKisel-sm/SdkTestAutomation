#!/usr/bin/env python3

from typing import Dict, Any
from conductor.client.http.api.workflow_resource_api import WorkflowResourceApi
from ..operation_utils import execute_with_error_handling, create_sdk_configuration
from ..sdk_response import SdkResponse


def execute(operation: str, parameters: Dict[str, Any]) -> SdkResponse:
    """Execute workflow operations with centralized error handling"""
    return execute_with_error_handling(lambda: _execute_operation(operation, parameters))


def _execute_operation(operation: str, parameters: Dict[str, Any]) -> SdkResponse:
    """Execute the specific workflow operation"""
    api_client = create_sdk_configuration()
    workflow_api = WorkflowResourceApi(api_client)
    
    if operation == "get-workflow":
        return _get_workflow(parameters, workflow_api)
    else:
        raise ValueError(f"Unknown workflow operation: {operation}")


def _get_workflow(parameters: Dict[str, Any], workflow_api: WorkflowResourceApi) -> SdkResponse:
    """Get workflow execution status"""
    workflow_id = parameters.get("workflowId", "")
    workflow = workflow_api.get_execution_status(workflow_id)
    return SdkResponse.create_success(workflow) 