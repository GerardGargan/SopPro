# SopPro - Standard Operating Procedure (SOP) Generator

## Overview

SopPro is a mobile application designed to streamline the creation and management of Standard Operating Procedures (SOPs). This app allows users to create, edit, review, and approve SOPs with step-by-step instructions, including text and images. The app supports version history, AI-based SOP generation, and offline functionality, making it a comprehensive solution for organisations to ensure standardisation and compliance across their operations.

### Features
- **SOP Generation and Management**: Add, update, and delete SOPs and SOP steps with text and images.
- **Add important safety information**: Add hazards, risks and required Personal Protective Equipment to SOPs.
- **Custom Templates and export**: Choose specific layouts and export SOPs to PDF in a template format with custom branding options.
- **AI-Based SOP Generation**: Automatically generate SOPs based on task description.
- **Image-Based Step Creation**: Attach images to SOP steps to enhance clarity.
- **Review and Approval**: SOPs can be reviewed and approved by designated users.
- **Version History**: Track changes and revert to previous versions of SOPs. Each version has a status (Draft, In Review, Approved, Rejected).
- **Multi-Tenancy Support**: Manage SOPs across different organisational tenants.

### Image Storage
The app uses **Azure Blob Storage** for storing and retrieving images associated with SOP steps.

## Tech Stack

- **Frontend**: 
  - React
  - React Native
  - Expo & Expo Router
  - Redux & Redux Toolkit
- **Backend**: 
  - .NET Core API
  - Entity Framework Core
  - Azure Blob Storage
- **Authentication**:
  - JSON Web Tokens (JWT)
  - .NET Identity framewrk

## Installation

### Prerequisites
- Install [Node.js](https://nodejs.org/)
- Install [Expo CLI](https://docs.expo.dev/get-started/installation/)
- Install [Visual Studio Code](https://code.visualstudio.com/) for development or VS Code
- Install .NET 9 SDK [.NET 9 Download](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- Set up an Azure Blob Storage account

### Running the App
1. Clone the repository:
   ```bash
   git clone https://github.com/gerardgargan/SopPro.git
2. Install dependencies for the mobile app:
   ```bash
   cd SopPro
   npm install
3. Add a.env file in the SopPro folder with the ip address of your backend .NET API
   ```bash
   EXPO_PUBLIC_API_URL=http://192.168.1.47:5000/api
4. Run the expo app
   ```bash
   npx expo start
5. Amend appsettings.json with a connection string to your SQL database, you will also need to update and provide values and tokens for Postmark (email) service, OpenAI and Azure Blob Storage.
   For security reasons, the actual values used for appsettings in this project have been git ignored.
6. Run the .NET API
   ```bash
   cd Backend
   dotnet run
7. Ensure your SQL server is running (via docker on Mac or via SQL server on windows)
   
