# Set Up Your AI Tutor

This project comes with an AI tutor in VS Code, powered by Gemini. It is set up to
**help you understand, not to write your code for you.** Ask it why something works,
what an error means, or where to look. It will answer with hints and questions, and
you do the typing.

First connect Unity to VS Code: in Unity, **Edit → Preferences → External Tools**, set
**External Script Editor** to **Visual Studio Code**, click **Regenerate project files**,
then double-click any script in `Assets/Scripts/`.

### Step 1: Install the Extension
Open this project folder in VS Code. A popup offers to install the recommended
extensions — click **Install**. You get **Unity** (autocomplete and debugging) and
**Google Antigravity** (your AI tutor).

Wait for VS Code to finish loading, then close VS Code. The extension needs to run once
before the next step works.

### Step 2: Sign In From a Terminal
Signing in from the Antigravity panel inside VS Code is broken right now: the
browser ends on a page that says **"localhost refused to connect."** Sign in from a
terminal instead.

1. Make sure VS Code is closed.
2. Open **PowerShell** and run:
   ```powershell
   & "$env:USERPROFILE\.gemini\bin\agy.exe"
   ```
3. Your browser opens. Sign in with your Google account and click **Sign in** on the
   "Make sure that you downloaded this app from Google" screen.
4. When the browser says you're done, go back to the terminal and close it.
5. Open VS Code. The Antigravity panel is now signed in.

### Step 3: Check That It's in Tutor Mode
In the Antigravity panel, ask: **"Write the code for a new food type worth 50 points for me."**

It should say no and offer a hint or a question instead. If it writes the code, tell
your instructor.

### Using Your Tutor Well
- **Say what you expected and what happened.** "I set `growthAmount` to 5 on the
  FoodPellet prefab, but my snake still grows by one segment. I expected five."
- **Paste the exact error** from Unity's Console. Don't paraphrase it.
- **Ask why, not what.** "Why does the `for` loop in `CreateStartingBody()` start `i` at 0?"
  teaches you more than "fix this."
- **Your features are your assignment.** The tutor will help you understand the loops and
  Lists already in the scripts, but it won't write your new food types, power-ups, AI
  behaviors, or other extensions for you.
  It won't answer the `// Why...?` questions in the scripts either. Those are yours to work out.
