# RepoInsight  

## Description  
RepoInsight is a tool designed to analyze JavaScript/TypeScript files in a repository and generate statistics on letter frequency, sorted in descending order.  

---

## Features  
- Analyze JavaScript/TypeScript files for letter frequency.  
- Generate detailed statistics sorted in descending order.  
- Easy setup and deployment using Docker Compose or manual configuration.
- Can run without a database.

---

## Prerequisites  
- **Docker and Docker Compose** (for Docker setup).  
- A valid **GitHub token** with repository read access.  
- Azure AD credentials for authentication.  
- **Node.js** and **.NET SDK** (for manual setup).  

---

## Running the Application  

### Using Docker  

1. **Clone the repository:**  
   ```bash  
   git clone https://github.com/your-username/repoinsight.git  
   cd repoinsight  
   ```  

2. **Set environment variables:**  
   Update the `docker-compose.yml` file with the required environment variables:  
   ```yaml  
   environment:  
     - ASPNETCORE_ENVIRONMENT=Development  
     - ConnectionStrings__DefaultConnection=Host=repoinsight_postgres;Database=repoinsight_db;Username=admin;Password=admin  
     - Github__Token=your_github_token_here  
     - AzureAd__Instance=https://login.microsoftonline.com/  
     - AzureAd__Domain=your_domain_here  
     - AzureAd__TenantId=your_tenant_id_here  
     - AzureAd__ClientId=your_client_id_here  
     - AzureAd__ClientSecret=your_client_secret_here
     - REACT_APP_CLIENT_ID: "your_client_id_here"
     - REACT_APP_AUTHORITY: "https://login.microsoftonline.com/your_tenant_id_here"  
   ```  

3. **Run the application:**  
   Use Docker Compose to build and start the app:  
   ```bash  
   docker-compose up --build  
   ```  

4. **Access the application:**  
   - Frontend: [http://localhost:3000](http://localhost:3000)
   - Backend: [http://localhost:8081/swagger](http://localhost:8081/swagger)

---

### Without Docker  

1. **Backend Setup:**  
   - Create an `appsettings.Development.json` file in the backend project directory with the following structure:  
     ```json  
     {  
       "ConnectionStrings": {  
         "DefaultConnection": "Host=localhost;Database=repoinsight_db;Username=admin;Password=admin"  
       },  
       "Github": {  
         "Token": "your_github_token_here"  
       },  
       "AzureAd": {  
         "Instance": "https://login.microsoftonline.com/",  
         "Domain": "your_domain_here",  
         "TenantId": "your_tenant_id_here",  
         "ClientId": "your_client_id_here",  
         "ClientSecret": "your_client_secret_here"  
       }  
     }  
     ```  
   - Run the backend using the .NET CLI:  
     ```bash  
     dotnet run  
     ```  

2. **Frontend Setup:**  
   - Navigate to the `frontend` directory.  
   - Create a `.env` file with the following variables:  
     ```env  
     REACT_APP_STATS_ENDPOINT=https://localhost:7277/api/Stats
     REACT_APP_CLIENT_ID=your_client_id_here  
     REACT_APP_AUTHORITY="https://login.microsoftonline.com/your_tenant_id_here"    
     REACT_APP_REDIRECT_URI=http://localhost:3000  
     ```  

   - Install dependencies:  
     ```bash  
     npm install  
     ```  

   - Start the development server:  
     ```bash  
     npm start  
     ```  

3. **Access the application:**  
   - Frontend: [http://localhost:3000](http://localhost:3000)
   - Backend: [http://localhost:7277/swagger](http://localhost:7277/swagger)

---

## Accessing the App  

To access the app on the website [https://repoinsight.cboutros.dev](https://repoinsight.cboutros.dev), you need an account. Please **contact me** to create your account.

---

## Environment Variables  

Refer to the **Docker** and **Without Docker** sections for the required variables.  

---

## Tech Stack  
- **Backend:** .NET Core  
- **Frontend:** React  
- **Database:** PostgreSQL  
- **Authentication:** Azure AD  

---

## Contributing  
1. Fork the repository.  
2. Create a new branch for your feature or bug fix.  
3. Commit your changes and push the branch.  
4. Open a pull request with a detailed description.  

---

## License  
This project is licensed under the MIT License. See the [`LICENSE`](LICENSE) file for details.  

---

Let me know if you'd like any further adjustments!
