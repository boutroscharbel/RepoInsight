Here’s a structured README for your `.NET React` app, **RepoInsight**:

---

# RepoInsight  

## Description  
RepoInsight is a tool designed to analyze JavaScript/TypeScript files in a repository and generate statistics on letter frequency, sorted in descending order.  

---

## Features  
- Analyze JavaScript/TypeScript files for letter frequency.  
- Generate detailed statistics sorted in descending order.  
- Easy setup and deployment using Docker Compose.  

---

## Prerequisites  
- **Docker and Docker Compose** installed on your system.  
- A valid **GitHub token** with repository read access.  
- Azure AD credentials for authentication.  

---

## Running the Application  

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
   ```  

3. **Run the application:**  
   Use Docker Compose to build and start the app:  
   ```bash  
   docker-compose up --build  
   ```  

4. **Access the application:**  
   The app will be available at [http://localhost:3000](http://localhost:3000).  

---

## Environment Variables  

- **ASPNETCORE_ENVIRONMENT:** Set to `Development` for local development.  
- **ConnectionStrings__DefaultConnection:** Connection string for the PostgreSQL database.  
- **Github__Token:** Your GitHub token for API access.  
- **AzureAd__Instance:** Azure AD login URL (default: `https://login.microsoftonline.com/`).  
- **AzureAd__Domain:** Your Azure AD domain.  
- **AzureAd__TenantId:** Azure AD tenant ID.  
- **AzureAd__ClientId:** Azure AD client ID.  
- **AzureAd__ClientSecret:** Azure AD client secret.  

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
This project is licensed under the [LICENSE NAME] License. See the `LICENSE` file for details.  

--- 

Let me know if you'd like to customize any part further!
