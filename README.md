# 📂 Bin_Obj_Delete_Project

## 1. 프로젝트 개요
Bin_Obj_Delete_Project는 지정한 경로에서 **bin/obj 등 여러 폴더와 특정 확장자를 가진 파일들**을   
**탐색 → 필터링 → 정렬 → 삭제 → 복원**까지 수행할 수 있는 WPF 기반 폴더·파일 관리 프로그램입니다.  

MVVM 패턴을 적용하여 **UI와 비지니스 로직을 분리**하였으며,  
**대용량 파일 처리** 환경에서도 안정적으로 동작하도록 설계되었습니다.  

주요 기능은 **폴더/파일 열거, 조건부 삭제, 휴지통 복원, 페이징 및 정렬, 진행률 표시** 등을 포함합니다.

---

## 2. 주요 기능

### 📂 폴더 및 파일 열거
- 지정 경로 또는 대화상자를 통해 폴더 경로 선택
- 하위 폴더와 파일을 재귀적으로 탐색
- 진행률 표시와 함께 UI에 실시간 반영
- 폴더 크기(바이트 단위) 계산 지원

### 🔍 필터 기능
- **폴더명 필터** : 콤마(,)로 구분된 이름 목록으로 필터링
- **확장자 필터** : `.png`, `.jpg`, `.exe` 형식으로 다중 확장자 지정 가능
- 필터 초기화 기능 제공 (UI 버튼/단축키)

### ↕ 정렬 기능
- 정렬 기준 : 이름, 생성일, 수정일, 크기, 경로, 유형
- 클릭 시 오름차순 / 내림차순 전환

### 🗑 삭제 기능
- **선택 삭제** : 선택한 항목만 휴지통 이동 또는 영구 삭제
- **전체 삭제** : 표시된 모든 항목 삭제
- 진행률 표시 지원
- 삭제 후 UI 자동 갱신

### ♻ 휴지통 복원
- 삭제 내역에서 선택 항목을 원래 경로로 복원
- 복원 후 목록에 즉시 반영

### 📄 페이징 기능
- 페이지 당 표시 개수 지정 (기본 100개)
- 이전/다음 페이지 이동 지원

### 📄 작업 로그 기록 (MSSQL)
- 삭제 및 복원 작업 결과를 DB에 기록
- 작업 대상 정보(INFO) 기록
  - ID (번호)
  - ACTION (삭제/복원)
  - ITEM (폴더/파일)
  - NAME (폴더명/파일명)
  - PATH (폴더 경로/파일 경로)
  - SIZE (바이트 단위)
  - IS_ERROR (에러 유무)
  - RESULT (성공/실패)
- 감사(Audit) 및 이력 추적 기능

---

## 3. 내부 구조 (MainVM 중심)

### MainVM
MVVM 패턴의 ViewModel로서 UI 상태와 비즈니스 로직 연결

#### 의존성 주입 서비스
- IEnumerateService : 폴더/파일 탐색 및 정보 수집
- IDeleteService : 항목 삭제 (휴지통 / 영구)
- IRecycleBinService : 휴지통 복원
- IAuditService : MSSQL 기반 작업 로그 기록

#### 상태 관리 프로퍼티
- **경로**
  -> DeleteFolderPath
- **필터**
  -> FilterFolderName, FilterExtensions
- **진행률**
  -> ProgressValue, ProgressStyle
- **데이터 컬렉션**
  -> LstAllData (전체 데이터)
  -> DeleteFolderInfo (삭제 데이터)
  -> ActiveFolderInfo (현재 UI 표시 데이터)
  -> SelectFolderInfo (선택 항목 데이터)

#### ICommand 바인딩
- **폴더 선택**
  -> LoadingFolderCommand
- **경로 입력**
  -> EnterLoadPathCommand
- **삭제 기능**
  -> DelSelMatchesCommand, DelAllMatchesCommand
- **필터 리셋**
  -> FilterResetFNCommand, FilterResetFECommand
- **복원 기능**
  -> RestoreFromRecycleBinCommand
- **기타**
  -> 정렬 / 페이징 관련 기타 커맨드

---

## 4. 동작 흐름
1. **폴더 선택**
   - Dialog 또는 직접 입력 -> 경로 설정
   - 기존 데이터 초기화
2. **탐색 수행**
   - 비동기 폴더 탐색 (EnumerateFolders)
   - 진행률 UI 반영
3. **필터 적용 및 데이터 구성**
   - 조건에 맞는 데이터 UI 표시
   - ObservableCollection 바인딩
4. **페이징 처리**
   - 페이지 단위 데이터 구성
   - ActiveFolderInfo 업데이트
5. **삭제 처리**
   - DeleteService 호출
   - 삭제 성공 항목만 UI 제거
   - MSSQL DB 로그 기록
7. **복원 처리**
   - RecycleBinService 호출
   - 원래 경로로 복원
   - UI 동기화
   - MSSQL DB 로그 동기화

---

## 5. 기술 스택
- **언어/프레임워크** : C#, WPF (.NET)
- **UI 패턴** : MVVM

## 6. 주요 사용 기술
1. **Microsoft.WindowsAPICodePack.Dialogs**
 -> 폴더 선택 UI
2. **ObservableCollection<T>**
 -> UI 데이터 바인딩
3. **RelayCommand/AsyncRelayCommand**
 -> MVVM 패턴 Command 처리
4. **Shell.Application (COM)**
 -> 휴지통 접근 및 복원
5. **MSSQL (ADO.NET)**
 -> 작업 로그 기록
    
---

## 7. 기술적 도전 & 해결
- **대용량 디렉터리 탐색 속도 최적화**
  - Directory.EnumerateDirectories 사용
  - 메모리 사용 최소화
- **UI 끊김 최소화**
  - 비동기 처리 + 프로그레스 분리
  - Dispatcher -> UI 업데이트
- **휴지통 복원 구현**
  - Windows Shell COM 인터페이스 분석
  - 원본 경로 기반 매칭 로직 구현
- **페이징 처리**
  - 페이지 단위 데이터 로딩
  - 대량 데이터 처리로 UI 성능 유지
- **예외 처리 강화**
  - 접근 권한 오류
  - 파일 잠금 상태
  - 경로 길이 문제
  - 네트워크 경로 예외 처리
- **작업 로그 시스템 구축**
  - MSSQL DB 연동
  - 삭제/복원 && 성공/실패 등 데이터 기록
  - 작업 이력 추적 가능
   
## 8. 성과
  - **수백 ~ 수천 개** 폴더/파일을 UI 지연 없이 처리
  - 반복적인 수동 삭제 작업 대비 **80% 이상 시간 절감**
  - 실무 환경에서도 활용 가능한 수준의 **폴더/파일 관리 도구** 구현

## 9. 한줄 요약
 - **파일 시스템 처리** + **UI** + **OS(Shell)** + **DB 로그**까지 통합한 실사용 가능 **미니 파일탐색기** 제작

---

## 7. 실행 방법
```bash
git clone https://github.com/사용자명/Bin_Obj_Delete_Project.git
