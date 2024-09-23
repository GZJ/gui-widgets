import sys
from PyQt5.QtCore import Qt, QEvent, QPoint
from PyQt5.QtWidgets import QApplication, QWidget, QVBoxLayout, QListWidget, QFrame, QListWidgetItem, QDesktopWidget

class MyWidget(QWidget):
    def __init__(self):
        super().__init__()
        self.initUI()

        self.dragging = False
        self.offset = QPoint()

    def initUI(self):
        self.setWindowTitle("gk-list") 
        self.setWindowFlags(Qt.FramelessWindowHint)

        desktop = QDesktopWidget()
        screen_rect = desktop.screenGeometry()
        screen_width = screen_rect.width()
        screen_height = screen_rect.height()

        window_width = int(screen_width / 3)  
        window_height = int(screen_height / 3)

        x = int((screen_width - window_width) / 2)
        y = int((screen_height - window_height) / 2)

        self.setGeometry(x, y, window_width, window_height)

        self.frame = QFrame(self)
        self.frame.setGeometry(0, 0, self.width(), self.height())

        layout = QVBoxLayout(self.frame)
        layout.setContentsMargins(0, 0, 0, 0)

        self.list_widget = QListWidget()
        self.list_widget.setStyleSheet("""
        QListView {
            outline: 0;
        }

        QListWidget {
            border : 1px solid green;
            background : black;
        }

        QListWidget QScrollBar {
            background : black;
            color: green;
        }
        QListWidget QScrollBar:vertical {
            background-color: green;
        }
        QListWidget QScrollBar::handle:vertical {
            background-color: green;
        }
        QScrollBar::add-page:vertical, QScrollBar::sub-page:vertical {
            background: none;
        }
        QScrollBar::sub-page:vertical {
            background: black;
        }
        QScrollBar::add-page:vertical {
            background: black;
        }

        QListWidget::item {
            border : 2px solid black;
            color: green;
        }
        QListWidget::item:selected {
            border-style: none;
            border : 2px solid black;
            background : green;
            color: black;
        }
        """)

        layout.addWidget(self.list_widget)
        self.setLayout(layout)

        data = self.get_data()
        for item_text in data:
            item = QListWidgetItem(item_text)
            item.setTextAlignment(Qt.AlignCenter)
            self.list_widget.addItem(item)

        self.list_widget.installEventFilter(self)
        self.list_widget.viewport().installEventFilter(self)  # 添加这一行

        self.list_widget.setCurrentRow(0)

    def eventFilter(self, source, event):
        if event.type() == QEvent.MouseButtonPress:
            if event.button() == Qt.LeftButton:
                self.dragging = True
                self.offset = event.pos()
        elif event.type() == QEvent.MouseMove:
            if self.dragging:
                self.move(self.mapToParent(event.pos() - self.offset))
        elif event.type() == QEvent.MouseButtonRelease:
            if event.button() == Qt.LeftButton:
                self.dragging = False
        elif event.type() == QEvent.KeyPress:
            key = event.key()
            if key == Qt.Key_J:
                self.move_selection(1)
                return True
            elif key == Qt.Key_K:
                self.move_selection(-1)
                return True
            elif key == Qt.Key_Q:
                QApplication.quit()
                return True
            elif key == Qt.Key_Return or key == Qt.Key_Enter:
                self.handle_enter()
                return True
        return super().eventFilter(source, event)

    def move_selection(self, direction):
        current_row = self.list_widget.currentRow()
        total_rows = self.list_widget.count()
        new_row = (current_row + direction) % total_rows
        self.list_widget.setCurrentRow(new_row)

    def handle_enter(self):
        selected_item = self.list_widget.currentItem()
        if selected_item:
            selected_text = selected_item.text()
            print(selected_text)
            QApplication.quit()

    def get_data(self):
        data = []
        if len(sys.argv) > 1:
            for arg in sys.argv[1:]:
                data.extend(arg.splitlines())
        else:
            data = sys.stdin.read().splitlines()
        return data

if __name__ == '__main__':
    app = QApplication(sys.argv)
    widget = MyWidget()
    widget.show()
    sys.exit(app.exec_())
