from revChatGPT.V1 import Chatbot

chatbot = Chatbot(config={
    "access_token": "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCIsImtpZCI6Ik1UaEVOVUpHTkVNMVFURTRNMEZCTWpkQ05UZzVNRFUxUlRVd1FVSkRNRU13UmtGRVFrRXpSZyJ9.eyJodHRwczovL2FwaS5vcGVuYWkuY29tL3Byb2ZpbGUiOnsiZW1haWwiOiJ0aWFuYnJlemFAZ21haWwuY29tIiwiZW1haWxfdmVyaWZpZWQiOnRydWUsImdlb2lwX2NvdW50cnkiOiJJVCJ9LCJodHRwczovL2FwaS5vcGVuYWkuY29tL2F1dGgiOnsidXNlcl9pZCI6InVzZXItcjJZMnJic0Y0ZmFvbkV4a1BvREtaOEo2In0sImlzcyI6Imh0dHBzOi8vYXV0aDAub3BlbmFpLmNvbS8iLCJzdWIiOiJnb29nbGUtb2F1dGgyfDEwNDEyODA5NjQ2ODgyNDg2NjQ0NyIsImF1ZCI6WyJodHRwczovL2FwaS5vcGVuYWkuY29tL3YxIiwiaHR0cHM6Ly9vcGVuYWkub3BlbmFpLmF1dGgwYXBwLmNvbS91c2VyaW5mbyJdLCJpYXQiOjE2Nzc1Nzk2MjUsImV4cCI6MTY3ODc4OTIyNSwiYXpwIjoiVGRKSWNiZTE2V29USHROOTVueXl3aDVFNHlPbzZJdEciLCJzY29wZSI6Im9wZW5pZCBwcm9maWxlIGVtYWlsIG1vZGVsLnJlYWQgbW9kZWwucmVxdWVzdCBvcmdhbml6YXRpb24ucmVhZCBvZmZsaW5lX2FjY2VzcyJ9.JjYL6_xCPzEmwnFG3SXMa-XqlJQnPlaNM7XcoC-82JEeG5Hjm9vr-zLoBdOaoooKhg9nvRWGC66t5dnu0G65zsXqAjgi6k1MjXVvi65tgf7bn5lyw-1--Rq59aYBKFzmwh7lxdOnLNIyOjeLkrMoEClksjn4KbWsef39X9nPgmT9HNcoZ3h5bKYdVNkTAUmCle3GYaf77im5Ava-f29PZ-cIuHrXb6euJBat1WWm9R1CPpyErRbodG27NbH7za7oEJwFIXJZd8Z962nVIkKNGiVD0tcpSJ24WFdOAC5CASGiKVpkwdSlm3NEwmp6i-TEMPtArGE_gm3hHHMuA5JZeQ"
})

print("Chatbot: ")
prev_text = ""
for data in chatbot.ask(
    "hows it going babe"
):
    message = data["message"][len(prev_text) :]
    print(message, end="", flush=True)
    prev_text = data["message"]
print()