# API Aggregation Service

## Overview

This service consolidates data from multiple external APIs and provides a unified endpoint to access the aggregated information.

## Endpoints

### GET /aggregation

Retrieve aggregated data from all integrated APIs.

#### Query Parameters

- `location` (string): The location for weather data.
- `query` (string): The search query for Twitter, News, Spotify, and GitHub data.

#### Response

```json
{
  "weatherData": "string",
  "twitterData": "string",
  "newsData": "string",
  "musicData": "string",
  "gitHubData": "string"
}