## Quick walkthrough of the App

[![Watch the demo](http://img.youtube.com/vi/3uQ3fFrEnnI/0.jpg)](https://youtu.be/3uQ3fFrEnnI)

## Overview of the system architecture

This Unity project uses the OpenAI Assistant API for interactive features, with the backend logic hosted on Buildship. The system architecture can be summarized as follows:

* Unity sends user messages directly to the Buildship server.
* Buildship connects to OpenAI Assistant.
* Firebase stores conversation history. For each unique conversation, a dedicated subcollection (within the main "threads" collection) is used to store all the messages and function calls exchanged in that particular interaction. This keeps each conversation's history separate and organized.
* Unity retrieves responses from Buildship.
* Unity displays messages or executes functions based on the AI's reply.


## Description of AI integration (API calls, prompt structure)

* OpenAI Assistant Prompt:

You are a friendly assistant providing information about a high-end speaker and its components.
Engage with the user in a welcoming and helpful manner. 

If the user's input is empty, offer a brief, practical insight or helpful detail related to the highlighted component: {component} to encourage engagement. Keep responses informative and to the point.

Respond to the user's queries with clear and focused information. Center your response around the highlighted component, {component} , when it is relevant to the user's query. Avoid repeating or restating which component is selected, as the user already knows. Provide concise and informative answers, staying on topic.

- **Tweeter**: A diamond-infused silk dome tweeter delivers crystal-clear highs with a smooth, natural tone. Its decoupled and rear-vented design reduces unwanted resonance, allowing delicate details to shine without harshness.
- **Crossover**: Using premium-grade components, the crossover cleanly splits frequencies at 300Hz and 3000Hz for a seamless blend between drivers. An adjustable tweeter attenuation switch lets you fine-tune the brightness to suit your environment.
- **Housing**: The bamboo laminate cabinet combines sustainable design with acoustic performance. Internal bracing minimizes unwanted vibrations, while a rear bass port enhances low-end response for a rich, full-bodied sound.
- **Woofer**: A woven flax composite woofer delivers warm, expressive mids and tight, controlled bass. It's finely tuned to maintain clarity in vocals and instruments while adding depth to the overall sound.
- **Subwoofer**: A down-firing, long-throw subwoofer provides deep, room-shaking lows with remarkable precision. Designed for clean power and presence, it anchors the soundstage with cinematic impact.

**Usage Recommendations**:
- **Home Theater**: Front/rear speakers for immersive sound; enhances movies/games.
- **Audiophile Listening**: Ideal for high-res audio; accurate music reproduction.
- **Studio Monitoring**: Near-field monitoring with flat frequency response.
- **Building Integration**: Flat back for custom wall integration; adjustable for room acoustics.
- **Installation**: Floor-standing unit, optimal when placed away from walls; gold-plated binding posts for connections; adjustable tweeter for room tuning.

Keep answers short and focused on explaining, clarifying, or enhancing the user's understanding of the selected component.


* API Calls

**Unity Request (JSON Payload):** When the user interacts, Unity sends a JSON payload to the Buildship server containing:
    **Highlighted Component:** Information about the currently selected or focused component in the Unity scene. This provides visual context to the Assistant.
    **User Message:** The text input or query from the user.
    **Thread ID:**
         The existing conversation thread ID if the interaction is a continuation of a previous dialogue.
         An empty string if it's the start of a new conversation.

 **Buildship Processing:** Upon receiving the JSON payload, the Buildship server performs the following:
     **Extracts Data:** Retrieves the highlighted component information, user message, and thread ID.
     **Retrieves Thread History (if applicable):** If a thread ID is provided, Buildship fetches all previously stored documents (messages and function calls) associated with that thread from the Firebase subcollection.
     **Sends to OpenAI Assistant:** Forwards the retrieved documents (conversation history), the highlighted component details, and the current user message to the OpenAI Assistant API.

 **OpenAI Assistant Response:** The OpenAI Assistant processes the information and sends back an API response to the Buildship server. This response includes:
     **Type:** Indicates the nature of the response, either `"message"` (a direct text reply) or `"function"` (an instruction to execute a specific function).
     **Value:** The actual content of the response. This will be the text of the message or the details (name and parameters) of the function to be called.

 **Buildship Saves Response:** The Buildship server then saves this entire API response (type and value) into the corresponding Firebase `"threadIds"` subcollection for the current conversation.

 **Unity Receives and Handles Response:** Unity subsequently queries the Buildship server (or is notified) about new data for the current thread ID. Upon receiving the response, Unity checks the `type`:
     If `type` is `"message"`, the `value` (the text) is displayed to the user.
     If `type` is `"function"`, the `value` (function name and parameters) is parsed, and the corresponding function is executed within the Unity application.


## Description of interaction logic

Users interact with the AI assistant primarily through a chat interface featuring an input field for text-based messages. Additionally, a dedicated button allows users to record their voice input, offering an alternative to typing.

To provide the AI with visual context, users can manipulate a 3D model of the speaker within the application. This includes the ability to:

 **Rotate:** Change the viewing angle of the speaker model.
 **Highlight:** Select or emphasize specific components of the speaker model.
 **Zoom In:** Magnify parts of the speaker model for closer inspection.

When a user sends a message (either typed or recorded via voice), the application captures the user's input. Crucially, it also identifies and includes information about any currently **highlighted component** of the 3D speaker model in the data sent to the Buildship server. This allows the AI to understand the user's query within the specific visual context they are focusing on. The user's message, along with a thread identifier (or an empty identifier for new conversations), is then transmitted to the Buildship backend.

The AI's response is processed, and if it's a textual message, it appears directly in the chat interface. If the AI returns a function call, this triggers actions within the 3D model or the application itself. For example, the AI might instruct the application to further highlight a specific part of the speaker, display information related to a component, or even guide the user through a specific interaction with the model. The ongoing conversation thread is managed using the thread ID, ensuring that the AI retains context across multiple questions and interactions related to the speaker model.


## Known issues and suggestions for improvements

**It is not always clear what actions users can perform within the application. This can lead to confusion and a poor user experience.
**Chatbot's timed tips and suggestions should appear in a separate UI and not in the chat.
**A future improvement could involve directly implementing Firebase for database management instead of relying on Buildship's collection system. Unity Scripts and a Buildship flow have been done to show some of this functionality
**Currently, function calls to OpenAI are immediately acknowledged as successful without waiting for actual execution and confirmation from Unity.
