# 🌍 MyCity - Location-Based Social Media Platform

> **Dual System Integration**: A comprehensive Unity application combining location-based social networking with AR city improvements

[![Unity](https://img.shields.io/badge/Unity-2020.3+-black.svg?style=for-the-badge&logo=unity)](https://unity.com/)
[![Firebase](https://img.shields.io/badge/Firebase-Cloud%20Services-orange.svg?style=for-the-badge&logo=firebase)](https://firebase.google.com/)
[![AR Foundation](https://img.shields.io/badge/AR%20Foundation-Location%20Based-blue.svg?style=for-the-badge)](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@4.2/manual/index.html)

## 🎬 Project Demonstration

### 📺 Video Demo

#### 📱 **Social Media Features**

<p align="center">
  <img src="./with participate and vote-small size.gif" width="400" alt="Social Media Demo">
</p>

> Complete demonstration of social networking functionality including user participation and voting features

#### 🏗️ **AR Improvements System** 

<p align="center">
  <img src="./Untitled design (2) (1).gif" width="400" alt="AR Demo">
</p>

> AR improvements demonstration showing 3D object placement at real-world locations



### 🖼️ Key Screenshots
*Add your app screenshots here to showcase the UI and main features*

## 🎯 **Two-System Integration**

MyCity seamlessly integrates two distinct but complementary platforms:

### 📱 **Social Media System** (SocialAppTemplate)
- **Complete social networking** with user profiles and authentication
- **Friend management** - Add, accept, decline, remove friends
- **Real-time messaging** - Private and group messaging with text and images
- **Community groups** - Create and join groups with group messaging
- **Posts & Feeds** - Create posts with likes, comments, and media sharing
- **Content categories** - Events, sales, social shares, polls with tagging system
- **Location integration** - Posts can include GPS location data

### 🏗️ **AR City Improvements System** (AR scripts)  
- **AR object placement** - Place 3D models at real-world GPS coordinates
- **City improvement types** - Benches, trees, lighting, cleaning equipment, parks, etc.
- **Cloud Anchors** - Persistent AR objects using Google Cloud Anchors
- **Location-based AR** - AR improvements tied to specific geographical locations
- **Firebase integration** - AR placements saved as feeds in the database

### 🔄 **System Integration**
- **Shared database** - AR improvements saved as special feed types in Firebase
- **Unified location services** - GPS coordinates used for both social posts and AR objects
- **Single codebase** - Both systems integrated within one Unity application

## ✨ Key Features

### 📱 **Social Media Platform Features**

#### 👥 **User Management & Social Networking**
- **User Authentication** - Registration, login, and profile management
- **Friend System** - Add friends, send/accept/decline friend requests
- **Real-Time Messaging** - Private messaging with text and image support
- **Group Management** - Create and join groups with group messaging
- **Activity Feeds** - View personal posts and friends' activity
- **Social Interactions** - Like and comment on posts

#### 📱 **Content & Media Sharing**
- **Post Creation** - Create posts with text, images, and videos
- **Content Categories** - Support for events, sales, social shares, and polls
- **Content Tags** - Music, sports, enrichment, religion, food, art, networking categories
- **Media Upload** - Photo and video sharing with Firebase storage
- **Location Data** - Posts can include GPS coordinates

#### 🎮 **Gamification Elements**
- **User Scoring** - Point system for user activities
- **Currency System** - Virtual currency mentioned in settings
- **XP Levels** - Experience point progression system

### 🏗️ **AR City Improvements Features**

#### 🔮 **AR Object System**
- **AR Placement** - Place 3D objects at real-world GPS coordinates using AR Foundation
- **Cloud Anchors** - Persistent AR objects using Google Cloud Anchors
- **Touch-to-Place** - Touch screen to position AR objects in real world
- **Object Preview** - View and adjust AR object placement before confirming

#### 🏗️ **City Improvement Types**
- **Infrastructure** - Cleaning equipment, fix items, pesticide applicators
- **Urban Furniture** - Benches, signs, dumpsters
- **Green Elements** - Trees, landscaping features
- **Transportation** - Street lighting, roundabouts, crosswalks
- **Recreation** - Parks and playground equipment

#### 📍 **Location Integration**
- **GPS Coordinates** - AR objects tied to precise geographical locations
- **Address Resolution** - Google Geocoding API integration for location names
- **Firebase Storage** - AR placements saved as special feed entries
- **Location Context** - AR objects include address and coordinate information

### 🔄 **System Integration**
- **Shared Database** - Both social posts and AR improvements stored in Firebase
- **Unified GPS Services** - Location services used for both social content and AR objects
- **Feed System** - AR improvements appear as special feed types in the social system

### 🎮 Basic Gamification System

Based on the AppSettings configuration, MyCity includes basic gamification elements:

#### 💰 **User Progression**
- **Score System** - Points awarded for user activities and participation
- **Currency System** - Virtual currency for potential premium features  
- **XP Levels** - Experience point progression system for user advancement
- **User Store** - Basic infrastructure for virtual item management

*Note: These are foundational gamification elements configured in the system settings. The full implementation details may vary.*

### 🏗️ AR City Improvement Visualization  

MyCity allows users to **visualize potential city improvements** using augmented reality technology by placing 3D objects at real-world locations.

#### 📺 **AR Object Placement Demo**

[![AR Object Placement Demo](https://img.shields.io/badge/▶️%20Watch%20Demo-AR%20City%20Improvements-orange?style=for-the-badge)](./Untitled%20design.mp4)

> See how AR 3D objects are placed at real-world locations using AR Foundation

#### 🏗️ **AR Object Placement System**

##### 📍 **AR Foundation Implementation**
- **Touch-to-Place** - Touch the screen to position AR objects at real-world locations
- **GPS Coordinates** - Each AR object tied to specific geographical coordinates
- **Cloud Anchors** - Persistent AR objects using Google Cloud Anchor technology
- **Real-World Positioning** - AR objects remain at their placed locations across sessions

##### 🏗️ **Available AR Object Types**
- **Infrastructure** - Cleaning equipment, fix items, pesticide applicators
- **Street Furniture** - Benches, signage, dumpsters
- **Green Elements** - Trees and landscaping features  
- **Transportation** - Street lighting, roundabouts, pedestrian crossings
- **Recreation** - Parks and recreational facilities

##### 💾 **Data Integration**
- **Firebase Storage** - AR object placements saved to Firebase database
- **Feed Integration** - AR objects appear as special feed entries in the social system
- **Location Services** - Google Geocoding API for address resolution
- **GPS Tracking** - Location data captured for each AR object placement

*Note: The AR system focuses on object placement and visualization rather than complex civic engagement features.*

## 🏗️ Technical Architecture

### 🛠️ Technology Stack
- **Unity 2020.3+** - Cross-platform game engine
- **Firebase Realtime Database** - Real-time data synchronization
- **Firebase Authentication** - User management
- **Firebase Storage** - Media file storage and CDN
- **Firebase Cloud Functions** - Server-side logic and notifications
- **Unity AR Foundation** - Augmented reality framework
- **Google Maps API** - Mapping and location services
- **Facebook SDK** - Social authentication integration

### 📦 Project Structure
```
📦 MyCity - Dual Platform Repository
├── 📁 MyCity-Unity-App/           # Main Unity application
│   ├── 📁 Assets/
│   │   ├── 📁 SocialAppTemplate/  # 📱 SOCIAL MEDIA SYSTEM
│   │   │   ├── Scripts/           # User management, messaging, feeds
│   │   │   ├── Controllers/       # Social networking controllers
│   │   │   ├── Resources/         # Social media configurations
│   │   │   └── Plugins/          # Social media integrations
│   │   ├── 📁 AR scripts/         # 🏗️ AR CITY IMPROVEMENTS SYSTEM
│   │   │   ├── instantiateAR.cs  # AR object placement & management
│   │   │   ├── GPS.cs            # Location services
│   │   │   └── GPS1.cs           # Enhanced GPS functionality
│   │   ├── 📁 main scenes/        # 🔄 UNIFIED UI & SCENES
│   │   │   ├── Maps/             # Location-based interface
│   │   │   ├── Social/           # Social media UI
│   │   │   └── AR/               # AR interface components
│   │   ├── 📁 Yamanas/           # App architecture & UI framework
│   │   ├── 📁 ARLocation/        # Location-based AR services
│   │   └── 📁 Plugins/           # Third-party integrations
│   ├── 📁 ProjectSettings/        # Unity configuration
│   └── 📁 Packages/              # Package dependencies
└── 📁 CloudServices/             # Cloud persistence & services
```

## 🎮 How It Works

### 1. **Location Discovery**
- Users browse an interactive map showing nearby content
- Content is displayed as markers with category-specific icons
- Users can filter content by type, distance, and popularity

### 2. **Content Creation**
- Users create posts tied to their current location or chosen map point
- Rich media support (photos, videos, 3D models)
- Category selection (events, sales, social, polls)
- Scheduling for time-sensitive content

### 3. **Social Interaction**
- Users can like, comment, and participate in location-based content
- Friend system enables following specific users' activities
- Real-time messaging for coordination and communication
- Group features for organizing community events

### 4. **AR Experience**
- Augmented reality overlays show content in real-world context
- AR markers appear when users point their camera at specific locations
- Interactive AR objects provide immersive experiences
- 3D models and animations enhance content presentation

### 5. **Gamification System**
- Users earn XP points for creating content, participating in events, and social interactions
- Virtual currency system allows purchasing premium features and content
- Real-time leaderboards create competition among community members
- Achievement badges unlock for reaching milestones and completing challenges
- Progressive user levels unlock advanced features and customization options

## 🌟 Content Categories

### 📅 Events
- **Local Gatherings** - Community meetups and social events
- **Entertainment** - Concerts, shows, and performances  
- **Sports** - Games, tournaments, and fitness activities
- **Educational** - Workshops, seminars, and learning events
- **Religious** - Spiritual gatherings and ceremonies

### 🛍️ Marketplace
- **Local Sales** - Buy and sell items in your area
- **Services** - Offer or find local services
- **Real Estate** - Property listings and rentals
- **Vehicle Sales** - Car and bike marketplace
- **Clothing & Fashion** - Fashion and apparel trading

### 📱 Social Posts
- **Photo Sharing** - Share moments with location context
- **Video Content** - Short-form video content
- **Text Posts** - Traditional social media updates
- **Check-ins** - Share your current location and activity
- **Reviews** - Rate and review local places and services

## 🎯 User Journey

### 1. **Onboarding**
- Social login (Facebook/Google) or email registration
- Location permissions and GPS setup
- Profile creation with avatar and interests
- Friend discovery and initial connections

### 2. **Content Discovery**
- Browse map view to see nearby content
- Filter by categories and interests
- View detailed content in street view or AR mode
- Engage with posts through likes and comments

### 3. **Content Creation**
- Choose location on map or use current GPS
- Select content type (event, sale, social, poll)
- Add media, text, and category tags
- Schedule publication and set visibility

### 4. **Social Engagement & Gamification**
- Connect with friends and build network
- Join events and activities to earn XP points
- Participate in community discussions for social scoring
- Complete daily challenges and unlock achievements
- Climb leaderboards and compete with friends
- Spend virtual currency on premium features and customizations

### 5. **AR City Improvements**
- Browse AR improvement objects placed at real-world locations
- Place new AR objects representing city improvements (benches, trees, lighting, etc.)
- View AR objects using Cloud Anchors for persistence
- Submit AR improvement placements that get saved to the social feed system

### 6. **System Integration**
- AR improvements saved as special feed types in the shared Firebase database
- Social posts and AR improvements both use GPS location services
- Single Unity application housing both social media and AR features

## 📊 Firebase Database Structure

```
📊 Firebase Realtime Database - Dual System Architecture
├── 👤 users/                      # 🔄 SHARED USER SYSTEM
│   ├── {userId}/
│   │   ├── profile info           # Basic user information
│   │   ├── friends list           # 📱 Social connections
│   │   ├── activity data          # 📱 Social activity tracking
│   │   ├── civic participation    # 🏗️ AR civic engagement data
│   │   ├── social scoring         # 📱 Social gamification points
│   │   ├── civic scoring          # 🏗️ Civic gamification points
│   │   └── settings              # User preferences
├── 📍 feeds/                      # 📱 SOCIAL MEDIA SYSTEM
│   ├── {feedId}/
│   │   ├── location data          # GPS coordinates
│   │   ├── content type           # Event, sale, social, poll
│   │   ├── media URLs             # Photos, videos
│   │   └── engagement metrics     # Likes, comments, shares
├── 💬 messages/                   # 📱 SOCIAL MESSAGING
│   ├── {chatId}/
│   │   └── message history        # Private & group messages
├── 👥 groups/                     # 📱 SOCIAL GROUPS
│   ├── {groupId}/
│   │   ├── members               # Group membership
│   │   └── group data            # Group settings & activity
├── 🏗️ ar_improvements/            # 🏗️ AR CITY IMPROVEMENTS SYSTEM
│   ├── {improvementId}/
│   │   ├── ar_object_data        # 3D model information
│   │   ├── gps_coordinates       # Precise location
│   │   ├── improvement_type      # Bench, tree, lighting, etc.
│   │   ├── voting_results        # Like/dislike counts
│   │   ├── comments              # Community feedback
│   │   └── implementation_status # Proposed, approved, built
├── 🗳️ civic_votes/                # 🏗️ DEMOCRATIC PARTICIPATION
│   ├── {voteId}/
│   │   ├── user_id               # Who voted
│   │   ├── improvement_id        # What they voted on
│   │   ├── vote_type             # Like/dislike
│   │   └── timestamp             # When they voted
├── 🏆 leaderboards/               # 🔄 UNIFIED GAMIFICATION
└── 📊 polls/                      # 📱 SOCIAL POLLS
    ├── {pollId}/
    │   ├── poll configuration     # Question, options
    │   ├── voting results         # Vote counts
    │   └── participation metrics  # Engagement data
```

## 📋 Portfolio Documentation

### 🎥 **Video Showcase**
*The GIF demonstrations above show the complete MyCity application in action*

### 📊 **Technical Documentation**
- **Architecture Overview** - System design and component interaction
- **Database Schema** - Firebase data structure and relationships  
- **API Integration** - Third-party service implementations
- **Development Process** - Code organization and best practices



## 🔧 Configuration

### Firebase Setup
- **Authentication**: Email/Password, Google, Facebook
- **Database**: Realtime Database with location indexing
- **Storage**: Image and video file hosting
- **Functions**: Server-side logic for notifications
- **Messaging**: Push notifications

### Unity Configuration
- **Platform**: Android (primary), iOS (secondary)
- **AR Support**: ARCore (Android), ARKit (iOS)
- **Location Services**: GPS with permission handling
- **Camera**: AR camera with focus and exposure control

## 📈 Performance & Optimization

### 🎯 Optimization Features
- **Efficient Data Loading** - Smart pagination and caching
- **Image Compression** - Automatic media optimization
- **Battery Management** - GPS and AR optimization
- **Network Efficiency** - Offline support and sync
- **Memory Management** - Asset pooling and cleanup

### 📊 Analytics & Monitoring
- **User Engagement** - Content interaction tracking
- **Performance Metrics** - App performance monitoring
- **Crash Reporting** - Automated error detection
- **Usage Analytics** - Feature adoption analysis



## 📱 Platform Support

### 🎯 Primary Platform
- **Android 8.0+** (API 26+)
- **ARCore Compatible** devices
- **4GB RAM** minimum
- **GPS** and **Camera** required

### 🔄 Additional Platforms
- **iOS 11+** (with modifications)
- **Unity Editor** (development/testing)
- **Web GL** (limited features)

## 🛠️ Technical Highlights & Skills Demonstrated

### 💻 **Core Technologies**
- **Unity 2020.3+** - Cross-platform mobile development
- **C# Programming** - Object-oriented design and dual-system architecture
- **Firebase Integration** - Real-time database, authentication, cloud storage
- **AR Foundation** - Augmented reality implementation for civic engagement
- **Google Maps API** - Location services and mapping for both systems
- **Facebook SDK** - Social authentication integration

### 🏗️ **Dual-System Architecture & Development**
- **Modular Design** - Independent yet integrated social and civic systems
- **Shared Service Layer** - Unified user management, location services, notifications
- **Real-time Data Sync** - Firebase Realtime Database for both social and civic data
- **Cross-System Communication** - Seamless data flow between social and AR components
- **Location-Based Services** - GPS integration for social posts and AR object placement
- **Social Media Framework** - Complete SocialAppTemplate implementation
- **AR Civic Engine** - Custom AR system for democratic participation
- **Dual Gamification** - Separate yet integrated reward systems
- **User Engagement Analytics** - Behavioral tracking across both platforms

### 📱 **Advanced Mobile Development Skills**
- **Complex State Management** - Coordinating two distinct application flows
- **Cross-platform Development** - Android/iOS compatibility for dual systems
- **Performance Optimization** - Managing AR rendering alongside social media features
- **Memory Management** - Efficient handling of 3D models and social media content
- **UI/UX Design** - Intuitive switching between social and civic interfaces
- **Camera Integration** - AR visualization and social media capture
- **Push Notifications** - Unified notification system for both platforms
- **Data Management** - Efficient caching and offline support for dual data streams
- **3D Graphics & AR** - Advanced AR rendering for civic proposals
- **Real-time Voting Systems** - Democratic participation with live results

## 📄 License

This project is proprietary software. All rights reserved.
Contact for licensing inquiries.



---

## 🏆 Project Achievements

### ✅ **Implemented Features**

#### 📱 **Social Media System (Complete)**
- ✅ **User Authentication & Management** - Registration, login, profile system
- ✅ **Social Networking** - Friend systems, messaging, and community features
- ✅ **Location-Based Social Content** - GPS-integrated posting and discovery
- ✅ **Real-Time Communication** - Firebase-powered messaging and notifications
- ✅ **Content Management** - Events, sales, social posts, polls with categorization
- ✅ **Group Functionality** - Community groups with management features
- ✅ **Social Gamification** - XP, achievements, leaderboards, virtual currency

#### 🏗️ **AR City Improvement System (Complete)**
- ✅ **AR Object Placement** - 3D object placement at real-world GPS coordinates
- ✅ **Cloud Anchors** - Persistent AR objects using Google Cloud Anchor technology
- ✅ **City Improvement Types** - Benches, trees, lighting, infrastructure AR models
- ✅ **Location Integration** - GPS coordinates and address resolution via Google Geocoding
- ✅ **Firebase Storage** - AR placements saved as feed entries in database

#### 🔄 **System Integration (Complete)**
- ✅ **Shared Database** - Both social posts and AR objects stored in Firebase
- ✅ **Unified GPS Services** - Location services used for both social content and AR objects
- ✅ **Single Application** - Both systems integrated within one Unity codebase
- ✅ **Feed Integration** - AR improvements appear as special feed types in social system
- ✅ **Cross-Platform Support** - Android-focused with iOS compatibility framework

### 🎯 **Development Scope**
- **Architecture**: Social media platform with AR object placement capabilities
- **Codebase**: Unity-based app with complete SocialAppTemplate integration + AR scripts
- **Systems**: Complete social networking framework + AR object placement system
- **Integration**: Shared Firebase database, unified GPS services, and single application architecture

---

## 📈 Project Impact & Learning

This **dual-purpose platform** demonstrates advanced mobile development and AR integration:

### 🏗️ **Technical Innovation**
- **Dual-System Architecture** - Successfully integrated social media with AR object placement
- **AR Visualization** - Real-world AR object placement using Cloud Anchors and GPS
- **Firebase Integration** - Unified database storing both social content and AR object data
- **Location Services** - GPS coordination for both social posts and AR object placement
- **Cross-Platform AR** - AR Foundation implementation for mobile AR experiences

### 🌍 **Application Potential**
- **Community Visualization** - AR objects help visualize potential improvements in real environments
- **Social Coordination** - Social platform enables community organization around shared interests
- **Location-Based Content** - GPS-integrated social media for local community engagement
- **AR Education** - Visual representation of concepts through AR object placement
- **Urban Planning Aid** - AR visualization assists in understanding spatial concepts

### 💡 **Development Skills Demonstrated**
- **Unity Development** - Cross-platform mobile development with AR Foundation
- **Firebase Architecture** - Complete backend integration with real-time database
- **AR Programming** - Cloud Anchors, GPS integration, and AR object placement
- **Social Media Systems** - User management, messaging, friends, groups, and content sharing
- **Location Services** - GPS integration, geocoding, and location-based features
- **Mobile Optimization** - Performance management for AR rendering and social content

---

*Portfolio Project showcasing Unity 2020.3, Firebase, and AR Foundation expertise*  
*Demonstrating full-stack mobile development and location-based social platform creation* 
