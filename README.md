# 📂 Bin_Obj_Delete_Project

## 1. 프로젝트 개요
Bin_Obj_Delete_Project는 지정한 경로에서 `bin` / `obj` 폴더 또는 특정 확장자 파일을 **탐색 → 필터링 → 정렬 → 삭제 → 복원**까지 수행할 수 있는 WPF 기반 폴더·파일 관리 프로그램입니다.  
MVVM 패턴을 적용하고, 주요 기능은 **폴더/파일 열거, 조건부 삭제, 휴지통 복원, 페이징 및 정렬, 진행률 표시** 등을 포함합니다.

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

---

## 3. 내부 구조 (MainVM 중심)

### MainVM
MVVM 패턴의 ViewModel로서 UI 상태와 비즈니스 로직 연결

#### 주요 의존성 주입 서비스
- `IEnumerateService` : 폴더/파일 탐색 및 정보 수집
- `IDeleteService` : 항목 삭제 (휴지통 / 영구)
- `IRecycleBinService` : 휴지통 복원

#### 상태 관리 프로퍼티
- 경로 : `DeleteFolderPath`
- 필터 : `FilterFolderName`, `FilterExtensions`
- 진행률 : `ProgressValue`, `ProgressStyle`
- 컬렉션 : `ActiveFolderInfo`, `DeleteFolderInfo`, `SelectFolderInfo`

#### 명령(ICommand) 바인딩
- `LoadingFolderCommand` — 폴더 대화상자 열기
- `EnterLoadPathCommand` — 경로 직접 입력
- `DelSelMatchesCommand`, `DelAllMatchesCommand` — 삭제 실행
- `FilterResetFNCommand`, `FilterResetFECommand` — 필터 초기화
- `RestoreFromRecycleBinCommand` — 복원 실행
- 정렬 및 페이징 관련 커맨드

---

## 4. 동작 방식 요약
1. **폴더 선택**
   - `CommonOpenFileDialog` 또는 직접 입력으로 경로 설정
   - 기존 목록 초기화
2. **열거 작업 실행**
   - 비동기 폴더 탐색 (`EnumerateFolders`)
   - 진행률 UI 업데이트
3. **필터 조건 적용 후 목록 표시**
   - `ObservableCollection` 바인딩
   - 페이지 단위로 `ActiveFolderInfo` 갱신
4. **삭제**
   - 선택/전체 삭제 시 `_deleteService.DeleteAsync` 호출
   - 삭제 성공 항목만 목록에서 제거
5. **복원**
   - `LstDelInfo` 내역 기반 복원 후 목록 재정렬

---

## 5. 기술 스택
- **언어/플랫폼** : C#, WPF (.NET)
- **UI 패턴** : MVVM
- **사용 라이브러리**
  - `Microsoft.WindowsAPICodePack.Dialogs` — 폴더 선택 대화상자
  - `ObservableCollection<T>` — UI 데이터 바인딩
  - Custom Command (`RelayCommand`, `AsyncRelayCommand`)

---

## 6. 기술적 도전 & 성과
- **대용량 디렉터리 탐색 속도 최적화**
  - `Directory.EnumerateDirectories` 활용으로 메모리 효율성 향상
  - 진행률 UI 업데이트와 탐색 로직을 분리하여 UI 끊김 최소화
- **휴지통 복원 기능 구현**
  - Win32 API / COM 인터페이스(IContextMenu) 분석 및 C# 적용
- **페이징 처리로 성능 최적화**
  - 페이지별 데이터 로딩으로 메모리 사용량 절감
- **예외 처리 강화**
  - 경로 접근 권한, 파일 잠김 상태, 네트워크 경로 예외 처리
- **성과**
  - 수백~수천 개 항목을 UI 끊김 없이 관리 가능
  - 수동 삭제 대비 작업 시간 **80% 이상 단축**
  - 업무 환경에서도 실사용 가능 수준 완성

---

## 7. 실행 방법
```bash
git clone https://github.com/사용자명/Bin_Obj_Delete_Project.git
