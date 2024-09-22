# TwitterCloneApi
A RESTful API built using .NET to simulate basic functionalities of Twitter, such as following/unfollowing users, posting tweets, liking posts, and more.

## Table of Contents
- [Overview](#overview)
- [Technologies](#technologies)
- [Features](#features)
- [API Endpoints](#api-endpoints)


## Overview
This project is a Twitter clone API built with .NET that allows users to register, follow/unfollow other users, post tweets, and like/dislike posts. Authentication is managed using JWT tokens.

## Technologies
- **.NET 8**
- **Entity Framework Core**
- **SQL Server**
- **JWT Authentication**
- **Swagger for API Documentation**

## Features
- **User Registration & Authentication**: Sign up and login using JWT-based authentication.
- **Follow/Unfollow Users**: Users can follow and unfollow each other.
- **Tweet Management**: Users can create, delete, and view tweets.
- **Likes**: Users can like/unlike posts.
- **Followers List**: Users can view their followers and who they are following.
- **Authorization**: Secure endpoints using authorization checks to ensure users can only access their own data or data they have permissions to view.
  
## API Endpoints

### Authentication
- **POST /api/auth/register** - Register a new user.
- **POST /api/auth/login** - Authenticate and get a JWT token.

### User Management
- **POST /api/userfollows/follow/{followerUsername}/{followedUsername}** - Follow a user.
- **DELETE /api/userfollows/unfollow/{followedUsername}** - Unfollow a user.
- **GET /api/userfollows/followers/{username}** - Get all followers of a user.
- **GET /api/userfollows/following/{username}** - Get all users that a user is following.

### Like Management
- **POST /api/tweets/{tweetId}/like** - Like a tweet.
- **DELETE /api/tweets/{tweetId}/like** - Unlike a tweet.

### Posts Management
- **GET /api/posts** - Retrieve all posts with likes, comments, and user details.
- **GET /api/posts/{Username}** - Retrieve all posts from a specific user.
- **POST /api/posts** - Create a new post for the authenticated user.
- **PUT /api/posts/{PostId}** - Update an existing post created by the authenticated user.
- **DELETE /api/posts/{PostId}** - Delete a post created by the authenticated user.
