#!/usr/bin/env python3

import json
from typing import Any, Optional


class SdkResponse:
    def __init__(self, success: bool, content: str = "", status_code: int = 200, data: Any = None, error_message: str = ""):
        self.status_code = status_code
        self.success = success
        self.data = data
        self.content = content
        self.error_message = error_message
    
    @staticmethod
    def create_success(data: Any = None) -> 'SdkResponse':
        """Create a success response with optional data"""
        content = ""
        if data is not None:
            try:
                if hasattr(data, 'to_dict'):
                    content = json.dumps(data.to_dict())
                elif isinstance(data, list):
                    content = json.dumps([item.to_dict() if hasattr(item, 'to_dict') else item for item in data])
                else:
                    content = json.dumps(data)
            except Exception:
                content = str(data)
        
        return SdkResponse(success=True, content=content, status_code=200, data=data)
    
    @staticmethod
    def create_error(status_code: int, error_message: str) -> 'SdkResponse':
        """Create an error response"""
        return SdkResponse(success=False, error_message=error_message, status_code=status_code)
    
    def to_dict(self):
        return {
            "statusCode": self.status_code,
            "success": self.success,
            "data": self.data,
            "content": self.content,
            "errorMessage": self.error_message
        } 