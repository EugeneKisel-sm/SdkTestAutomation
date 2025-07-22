from setuptools import setup, find_packages

setup(
    name="conductor-sdk-wrapper",
    version="1.0.0",
    description="CLI wrapper for Conductor Python SDK",
    author="SdkTestAutomation",
    packages=find_packages(),
    install_requires=[
        "conductor-python>=1.0.0",
        "click>=8.0.0",
    ],
    entry_points={
        "console_scripts": [
            "conductor-sdk-wrapper=sdk_wrapper.main:main",
        ],
    },
    python_requires=">=3.9",
) 