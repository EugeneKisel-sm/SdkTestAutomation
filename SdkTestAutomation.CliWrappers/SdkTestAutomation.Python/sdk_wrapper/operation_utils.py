#!/usr/bin/env python3

import os
from typing import Callable, Any
from conductor.client.http.api_client import ApiClient
from .sdk_response import SdkResponse


def execute_with_error_handling(operation: Callable[[], SdkResponse]) -> SdkResponse:
    """Execute an operation with centralized error handling"""
    try:
        return operation()
    except Exception as e:
        return SdkResponse.create_error(500, str(e))


def create_sdk_configuration() -> ApiClient:
    """Create SDK configuration with server URL from environment variable or default"""
    api_client = ApiClient()
    server_url = os.getenv('CONDUCTOR_SERVER_URL')
    if server_url:
        api_client.configuration.host = server_url
    return api_client 