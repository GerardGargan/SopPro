# SopPro - Standard Operating Procedure (SOP) Generator

## Overview

SopPro is a mobile application designed to streamline the creation and management of Standard Operating Procedures (SOPs). This app allows users to create, edit, review, and approve SOPs with step-by-step instructions, including text and images. The app supports version history, AI-based SOP generation, and offline functionality, making it a comprehensive solution for organisations to ensure standardisation and compliance across their operations.

### Features
- **SOP Generation and Management**: Add, update, and delete SOPs and SOP steps steps with text and images.
- **Add important safety information**: Add hazards, risks and required Personal Protective Equipment to SOPs.
- **Custom Templates and export**: Choose specific layouts and export SOPs to PDF in your chosen layout.
- **AI-Based SOP Generation**: Automatically generate SOPs based on predefined templates.
- **Image-Based Step Creation**: Attach images to SOP steps to enhance clarity.
- **Review and Approval**: SOPs can be reviewed and approved by designated users.
- **Version History**: Track changes and revert to previous versions of SOPs.
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

## Installation

### Prerequisites
- Install [Node.js](https://nodejs.org/)
- Install [Expo CLI](https://docs.expo.dev/get-started/installation/)
- Install [Visual Studio Code](https://code.visualstudio.com/) for development
- Set up an Azure Blob Storage account

### Running the App
1. Clone the repository:
   ```bash
   git clone https://github.com/yourusername/SopPro.git
2. Install dependencies:
   ```bash
   cd SopPro
   npm install
3. Add a.env file in the SopPro folder with the ip address of your backend .NET API
   ```bash
   EXPO_PUBLIC_API_URL=http://192.168.1.47:5000/api
5. Run the expo app
   ```bash
   npx expo start
6. Run the .NET API
   ```bash
   cd Backend
   dotnet run
7. Ensure your SQL server is running (via docker on Mac or via SQL server on windows)
8. Amend appsettings.json to point to your SQL server, azure blob etc.
   
