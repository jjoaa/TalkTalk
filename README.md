# TalkTalk
<br />

## 1. 소개
> C# Windows Forms 기반의 소켓 통신 프로그램으로, <br />
> 클라이언트와 서버가 각각 독립적인 WinForms 분리되어 있으며,  <br />
> TCP 소켓을 사용해 실시간 데이터 교환과 상호작용을 구현합니다.

<br /> 

- 단일 클라이언트 채팅 <br />  <br />
![GOMCAM 20250520_1904050364](https://github.com/user-attachments/assets/aa41cee6-4d84-46b6-8b7e-49009b8b3025)  <br />
 <br />
 
- 다중 클라이언트 채팅  <br />  <br />
![GOMCAM 20250521_1754080155](https://github.com/user-attachments/assets/7eff9357-36ee-47ad-a064-c12f51138502) <br />

<br /> 

### 작업기간
2025/05, 1주
<br /><br />

### 인력구성
1인
<br /><br /><br />

## 2. 기술스택

<img src ="https://img.shields.io/badge/C_sharp-1572B6.svg?&style=for-the-badge&logo=Csharp&logoColor=Blue"/>  <br /><br /> <br />


## 3. 프로젝트 구조 
두가지 버전의 채팅 프로그램을 포함하고 있습니다 <br />
| 폴더 이름                           | 설명                                                                                           |
| ------------------------------- | -------------------------------------------------------------------------------------------- |
| `Server` / `Client`             | **단일 클라이언트 채팅**을 위한 기본 버전입니다. 서버는 한 명의 클라이언트만 처리할 수 있습니다.                                    |
| `Multi-Server` / `Multi-Client` | **여러 명의 클라이언트가 동시에 채팅**할 수 있는 확장 버전입니다. 각 클라이언트는 닉네임을 설정할 수 있고, 서버는 멀티스레드를 사용해 모든 연결을 처리합니다. |

<br /><br /><br />


## 4. 기능
### 📂 Project Structure (폴더 구조)
```
TalkTalk/
├── server/ # 단일 클라이언트 채팅 지원 서버
│ ├── Form1.cs
│ ├── Form1.Designer.cs
│ ├── Program.cs
│ └── ...
│
├── client/ # 단일 클라이언트 채팅
│ ├── TalkTalk.sln
│ ├── Form1.cs
│ ├── Form1.Designer.cs
│ └── ...
│
├── Multi_server/ # 다중 클라이언트 채팅 지원 서버
│ ├── Form1.cs
│ ├── Form1.Designer.cs
│ ├── Program.cs
│ └── ...
│
├── Multi_client/ # 다중 클라이언트 채팅 
│ ├── TalkTalk.sln
│ ├── Form1.cs
│ ├── Form1.Designer.cs
│ └── ...
│
├── README.md
└── .gitignore

```
<br /><br />

## 5. 앞으로 학습할 것들, 나아갈 방향
- 사용자 인증 추가
- 데이터베이스 연동 (MySQL)
- <del>로그 및 예외 처리 모듈 추가 </del>
- UI/UX 개선
  
<br /><br /> <br /> 
