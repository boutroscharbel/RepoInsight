# Use the official Node.js image as the base image
FROM node:14

# Set the working directory
WORKDIR /app

# Copy the package.json and package-lock.json files
COPY package*.json ./

# Install dependencies
RUN npm install

# Copy the rest of the application code
COPY . .

# Build arguments to pass environment variables
ARG REACT_APP_STATS_ENDPOINT
ARG REACT_APP_CLIENT_ID
ARG REACT_APP_AUTHORITY
ARG REACT_APP_REDIRECT_URI

# Set environment variables
ENV REACT_APP_STATS_ENDPOINT=$REACT_APP_STATS_ENDPOINT
ENV REACT_APP_CLIENT_ID=$REACT_APP_CLIENT_ID
ENV REACT_APP_AUTHORITY=$REACT_APP_AUTHORITY
ENV REACT_APP_REDIRECT_URI=$REACT_APP_REDIRECT_URI

# Build the React app
RUN npm run build

# Install serve to serve the build
RUN npm install -g serve

# Set the command to serve the build
CMD ["serve", "-s", "build", "-l", "3000"]

# Expose the port the app runs on
EXPOSE 3000