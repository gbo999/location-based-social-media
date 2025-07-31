# 🎬 Adding Video to GitHub README Guide

## 🌟 **Best Options for MyCity Portfolio**

### 1. **YouTube Embedding (Recommended)**
```markdown
[![MyCity App Demo](https://img.youtube.com/vi/YOUR_VIDEO_ID/maxresdefault.jpg)](https://www.youtube.com/watch?v=YOUR_VIDEO_ID)
```

**Steps:**
1. Upload your video to YouTube
2. Get the video ID from URL: `youtube.com/watch?v=VIDEO_ID`
3. Replace `YOUR_VIDEO_ID` in the code above
4. Creates a clickable thumbnail that opens YouTube

### 2. **GitHub Video Upload (Direct)**
```markdown
https://user-images.githubusercontent.com/YOUR_USER_ID/VIDEO_FILE_NUMBER.mp4
```

**Steps:**
1. Create a new GitHub issue in your repo
2. Drag & drop your video file into the issue comment
3. GitHub will generate a URL like above
4. Copy that URL and use in README (you can delete the issue)
5. Supports MP4, MOV, WEBM up to 25MB

### 3. **Animated GIF Preview**
```markdown
![MyCity Demo](./assets/demo.gif)
```

**Steps:**
1. Convert your video to GIF (use tools like EZGIF, GIPHY)
2. Keep under 25MB for GitHub
3. Store in `/assets/` folder in your repo

### 4. **Cloud Storage Link**
```markdown
[🎥 Watch MyCity Demo](https://drive.google.com/file/d/YOUR_FILE_ID/view)
```

**Options:**
- Google Drive (make public)
- Dropbox 
- OneDrive
- Vimeo

## 📝 **For Your README**

Replace these placeholders in your README.md:

```markdown
### 📺 Video Demo
[![MyCity App Demo](https://img.youtube.com/vi/YOUR_VIDEO_ID/maxresdefault.jpg)](https://www.youtube.com/watch?v=YOUR_VIDEO_ID)
> Complete walkthrough of MyCity features including location-based posting, AR integration, and social interactions

### 📺 YouTube Showcase
[**📱 MyCity on YouTube**](https://www.youtube.com/watch?v=YOUR_VIDEO_ID)
> Detailed feature breakdown and development insights
```

## 🎯 **Best Practice for Portfolio**

**Recommended approach:**
1. **Upload to YouTube** - Professional, fast loading, good for CV
2. **Add GIF preview** - Shows action immediately in README
3. **Include direct link** - For easy access

Example:
```markdown
## 🎬 Project Demonstration

![MyCity Demo Preview](./assets/demo.gif)

### 📺 Full Demo Video
[![MyCity App Demo](https://img.youtube.com/vi/YOUR_VIDEO_ID/maxresdefault.jpg)](https://www.youtube.com/watch?v=YOUR_VIDEO_ID)

[🎥 **Direct YouTube Link**](https://www.youtube.com/watch?v=YOUR_VIDEO_ID)
``` 