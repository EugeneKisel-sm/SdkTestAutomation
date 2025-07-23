#!/usr/bin/env python3

import argparse
import json
import sys
import os

# Add the current directory to Python path to handle imports
current_dir = os.path.dirname(os.path.abspath(__file__))
if current_dir not in sys.path:
    sys.path.insert(0, current_dir)

from operations import event_operations, workflow_operations
from sdk_response import SdkResponse


def main():
    parser = argparse.ArgumentParser(description='Python Conductor SDK Test Wrapper')
    parser.add_argument('--operation', required=True, help='SDK operation')
    parser.add_argument('--parameters', required=True, help='JSON parameters')
    parser.add_argument('--resource', required=True, help='Resource type')
    
    args = parser.parse_args()
    
    try:
        params = json.loads(args.parameters)
        result = execute_operation(args.operation, params, args.resource)
        print(json.dumps(result.to_dict()))
    except Exception as e:
        error = SdkResponse.create_error(500, str(e))
        print(json.dumps(error.to_dict()))
        sys.exit(1)


def execute_operation(operation: str, parameters: dict, resource: str):
    """Execute the operation based on resource type"""
    if resource == "event":
        return event_operations.execute(operation, parameters)
    elif resource == "workflow":
        return workflow_operations.execute(operation, parameters)
    else:
        raise ValueError(f"Unknown resource: {resource}")


if __name__ == '__main__':
    main() 