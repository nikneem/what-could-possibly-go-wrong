# 20 Years of software development - What could possibly go wrong?

Every great developer starts with a blinking cursor and an overwhelming sense of curiosity. The first twenty years in software development are a journey of trial and triumph, where knowledge compounds like well-structured code and experience shapes not just technical skill but mindset.

From the wide-eyed junior, eager to write code but still learning the fundamentals, to the confident medior, who starts to see patterns and best practices emerge. The senior years bring architectural thinking and mentorship, bridging the gap between business and technology. And for those who persist, the path to principal leads to a place where impact extends far beyond code—shaping systems, strategies, and the next generation of engineers.

This is the story of that journey. A story of learning, failing, growing, and eventually leading. Not every developer follows the same path, but the lessons learned along the way are universal.

## VotR

Welcome to **VotR**, an interactive voting platform designed for conference sessions. VotR allows speakers to create engaging presentations by pre-defining sessions with multiple-choice questions and answer options. Attendees can vote live during the session, making talks more interactive and memorable.

This repository is part of the conference session **"50 Years of Development Experience: What Could Possibly Go Wrong"** by **Sander Molenkamp** and **Eduard Keilholz**.

---

## VotR Project Structure

VotR is developed in two distinct project structures:

Microservices Solution – A distributed architecture where different components of VotR are implemented as independent services.
Monolith – A single-application architecture where all features are contained within a unified codebase.
Choosing the Right Branch
To ensure you're working with the correct project structure, please check the appropriate Git branch:

[main branch](https://github.com/nikneem/what-could-possibly-go-wrong) – Contains the microservices-based implementation.  
[monolith branch](https://github.com/nikneem/what-could-possibly-go-wrong/tree/modular-monolith) – Contains the monolithic implementation.  
Make sure to switch to the correct branch in your version control system before exploring the code or making changes.

---

## 🎯 Features

- **Session Management**: Speakers can pre-define sessions, questions, and multiple-choice answer options.
- **Live Voting**: Attendees can cast votes in real-time.
- **Results Display**: Real-time results visualization during the session.
- **Scalable Design**: Built to handle hundreds of votes simultaneously.

---

## 🛠️ Tech Stack

VotR is built using a modern, scalable, and highly reliable tech stack:

- **C# & .NET Aspire**: Core application development.
- **Dapr (Distributed Application Runtime)**: Integration with state store, pub/sub messaging, and service invocation.
- **Azure Container Apps**: Hosting microservices with Dapr and scaling using KEDA.
- **SignalR**: Real-time communication for vote updates.
- **SQLite**: Lightweight database for local development.
- **Docker**: Containerized application setup for streamlined deployment.

---

## 🚀 Getting Started

### Prerequisites

1. [.NET Aspire](https://dotnet.microsoft.com/)
2. [Dapr CLI](https://docs.dapr.io/getting-started/install-dapr-cli/)
3. [Docker](https://www.docker.com/)
4. Node.js (for frontend development if applicable)

### Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/nikneem/what-could-possibly-go-wrong.git
   cd what-could-possibly-go-wrong
   ```
2. Build the solution:
   ```bash
   dotnet build
   ```

### Running Locally

1. Start the Aspire project:
   ```bash
   dotnet run
   ```

---

## 🧪 Testing

Unit and integration tests are included. Run tests with:

```bash
dotnet test
```

---

## 🤝 Contributing

This repository is part of a conference session and is not intended for production use. Contributions are welcome to enhance or experiment with features, but keep in mind this project was created for educational purposes.

---

## ⚠️ Disclaimer

VotR was built as a showcase for **"50 Years of Development Experience: What Could Possibly Go Wrong"**. It may include intentional architectural challenges or quirks to stimulate discussion during the session.

---

## 📝 License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

---

## 🙌 Acknowledgments

Thanks to Sander Molenkamp and Eduard Keilholz for their invaluable contributions and insights while designing this system.

---

Ready to make your conference sessions more interactive? 🎤✨ Let the votes begin!

---

Let me know if you'd like adjustments or additional details!
